using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.WebUI;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;
using GridDomain.Tests.Configuration;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using Quartz;
using Quartz.Spi;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    [TestFixture]
    public class Spec : NodeCommandsTest
    {
        private const string Id = "test";
        private const string Group = "test";
        private IActorRef _scheduler;
        private IScheduler _quartzScheduler;
        private Mock<IQuartzLogger> _quartzLogger;
        private IUnityContainer _container;

        public Spec() : base(new AutoTestAkkaConfiguration().ToStandAloneSystemConfig())
        {

        }

        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            _container = Register();
            var system = ActorSystemFactory.CreateActorSystem(akkaConf);
            system.AddDependencyResolver(new UnityDependencyResolver(_container, system));
            var router = new TestRouter();
            return new GridDomainNode(_container, router, TransportMode.Standalone, system);
        }

        private IUnityContainer Register()
        {
            var container = Container.CreateChildScope();
            container.RegisterType<QuartzJob>();
            container.RegisterType<ISchedulerFactory, SchedulerFactory>();
            container.RegisterType<IScheduler>(new InjectionFactory(x => x.Resolve<ISchedulerFactory>().GetScheduler()));
            var loggingJobListener = new Mock<ILoggingJobListener>();
            loggingJobListener.Setup(x => x.Name).Returns("testListener");
            container.RegisterInstance(loggingJobListener.Object);
            container.RegisterType<ILoggingJobListener, LoggingJobListener>();
            container.RegisterInstance(new Mock<ILoggingSchedulerListener>().Object);
            container.RegisterType<ILoggingSchedulerListener, LoggingSchedulerListener>();
            container.RegisterType<IQuartzConfig, QuartzConfig>();
            container.RegisterType<ActorSystem>(new InjectionFactory(x => GridNode.System));
            container.RegisterType<IWebUiConfig, WebUiConfig>();
            container.RegisterType<IWebUiWrapper, WebUiWrapper>();
            container.RegisterType<IJobFactory>(new InjectionFactory(x => new JobFactory(container)));
            _quartzLogger = new Mock<IQuartzLogger>();
            container.RegisterInstance(_quartzLogger.Object);
            container.RegisterType<SchedulingActor>();
            container.RegisterType<AggregateActor<TestAggregate>>();
            container.RegisterType<AggregateHubActor<TestAggregate>>();
            container.RegisterType<ICommandAggregateLocator<TestAggregate>, TestAggregateCommandHandler>();
            container.RegisterType<IAggregateCommandsHandler<TestAggregate>, TestAggregateCommandHandler>();
            ScheduledCommandProcessingSagaRegistrator.Register(container);
            return container;
        }

        [SetUp]
        public void SetUp()
        {
            CreateScheduler();
            _scheduler = GridNode.System.ActorOf(GridNode.System.DI().Props<SchedulingActor>());
            _quartzScheduler.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            ResultHolder.Clear();
            _quartzScheduler.Shutdown(true);
        }

        private void CreateScheduler()
        {
            _quartzScheduler = _container.Resolve<IScheduler>();
        }

        [Test]
        public void When_a_message_published_Then_saga_receives_it()
        {
            Thread.Sleep(1000);
            var testCommand = new SuccessCommand(Id, Group);
            _scheduler.Ask<Scheduled>(new Schedule(testCommand, DateTime.UtcNow.AddSeconds(1), Timeout)).Wait(Timeout);
            WaitFor<CompleteJob>();
        }

        [Test]
        public void When_system_resolves_scheduler_Then_single_instance_is_returned_in_all_cases()
        {
            var sched1 = _container.Resolve<IScheduler>();
            var sched2 = _container.Resolve<IScheduler>();
            var sched3 = _container.Resolve<IScheduler>();
            Assert.True(sched1 == sched2 && sched2 == sched3);
        }

        [Test]
        public void When_system_shuts_down_the_scheduler_Then_the_next_resolve_will_return_another_instance()
        {
            var sched1 = _container.Resolve<IScheduler>();
            sched1.Shutdown(false);
            var sched2 = _container.Resolve<IScheduler>();
            Assert.True(sched1 != sched2);
        }

        [Test]
        [Ignore]
        public void WebConsoleTest()
        {
            if (!Debugger.IsAttached)
            {
                Assert.True(true);
                return;
            }

            using (_container.Resolve<IWebUiWrapper>().Start())
            {
                var runAt = DateTime.UtcNow.AddSeconds(500);
                var testMessage = new SuccessCommand("web", "web");
                _scheduler.Ask<Scheduled>(new Schedule(testMessage, runAt, Timeout)).Wait(Timeout);
                _scheduler.Ask<Scheduled>(new Schedule(new SuccessCommand(Id, Group), DateTime.UtcNow.AddSeconds(10), Timeout)).Wait(Timeout);
                _scheduler.Ask<Scheduled>(new Schedule(new SuccessCommand(Id + Id, Group), DateTime.UtcNow.AddSeconds(15), Timeout)).Wait(Timeout);
                Throttle.Assert(() => Assert.True(ResultHolder.Contains("web")), maxTimeout: TimeSpan.FromHours(1));
            }
        }

        [Test]
        public void When_job_is_added_Then_it_gets_executed()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var testMessage = new SuccessCommand(Id, Group);
            _scheduler.Ask<Scheduled>(new Schedule(testMessage, runAt, Timeout)).Wait(Timeout);

            WaitFor<ScheduledCommandSuccessfullyProcessed>();
            Throttle.Assert(() => Assert.True(ResultHolder.Contains(testMessage.TaskId)), maxTimeout: Timeout);
        }

        [Test]
        public void When_scheduler_is_restarted_during_job_execution_Then_on_next_start_job_is_not_fired_again()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var testMessage = new TimeoutCommand(Id, Group, TimeSpan.FromSeconds(2));

            _scheduler.Ask<Scheduled>(new Schedule(testMessage, runAt, Timeout)).Wait(Timeout);
            WaitFor<CompleteJob>();
            _quartzScheduler.Shutdown(false);
            Thread.Sleep(1000);
            CreateScheduler();
            //it takes a lot of time to scheduler to actually fire job second time
            WaitFor<ScheduledCommandSuccessfullyProcessed>();
            Assert.True(ResultHolder.Count == 1 && ResultHolder.Contains(Id));
        }

        [Test]
        public void When_processing_actor_throws_Then_scheduler_receives_failure_response()
        {

            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var testMessage = new FailCommand(Id, Group);
            _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            //TODO::VZ:: to really test system I need a way to check that scheduling saga received the message
            WaitFor<ScheduledCommandProcessingFailed>();
        }

        [Test]
        public void When_there_are_several_scheduled_jobs_System_executes_all_of_them()
        {
            var tasks = new[] { 0.5, 0.6, 0.7, 0.8, 1 };

            foreach (var task in tasks)
            {
                var testMessage = new SuccessCommand(task.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            }

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() => ResultHolder.Contains(taskIds));
        }

        [Test]
        public void When_client_tries_to_add_two_task_with_same_id_Then_only_one_gets_executed()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var secondRunAt = DateTime.UtcNow.AddSeconds(1);
            var testMessage = new SuccessCommand(Id, Group);
            _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            _scheduler.Tell(new Schedule(testMessage, secondRunAt, Timeout));

            Throttle.Assert(() => Assert.True(ResultHolder.Count == 1), minTimeout: TimeSpan.FromSeconds(2));
        }

        [Test]
        public void When_some_of_scheduled_jobs_fail_System_still_executes_others()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var failTasks = new[] { 1.0, 2.0 };

            foreach (var task in successTasks)
            {
                var testCommand = new SuccessCommand(task.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new Schedule(testCommand, runAt, Timeout));
            }
            foreach (var failTask in failTasks)
            {
                var failTaskCommand = new FailCommand(failTask.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(failTask);
                _scheduler.Tell(new Schedule(failTaskCommand, runAt, Timeout));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var failTaskIds = failTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() =>
            {
                ResultHolder.Contains(successTaskIds);
                Assert.True(failTaskIds.All(x => ResultHolder.Get(x) == null));
            }, minTimeout: TimeSpan.FromSeconds(3));
        }

        [Test]
        public void When_tasks_get_deleted_after_scheduling_System_will_not_execute_them()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var tasksToRemove = new[] { 1.0, 2.0 };

            foreach (var task in successTasks.Concat(tasksToRemove))
            {
                var testMessage = new SuccessCommand(task.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var tasksToRemoveTaskIds = tasksToRemove.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            foreach (var taskId in tasksToRemoveTaskIds)
            {
                _scheduler.Tell(new Unschedule(taskId, "test"));
            }

            Throttle.Assert(() =>
            {
                ResultHolder.Contains(successTaskIds);
                Assert.True(tasksToRemoveTaskIds.All(x => ResultHolder.Get(x) == null));
            }, minTimeout: TimeSpan.FromSeconds(4));
        }

        [Test]
        public void When_scheduler_is_restarted_Then_scheduled_jobs_still_get_executed()
        {
            var tasks = new[] { 0.5, 1, 1.5, 2, 2.5 };

            foreach (var task in tasks)
            {
                var testMessage = new SuccessCommand(task.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            }

            _quartzScheduler.Shutdown(false);
            Thread.Sleep(2000);
            CreateScheduler();

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() => ResultHolder.Contains(taskIds));
        }

        protected override TimeSpan Timeout
        {
            get
            {
                //default timeout
                var timeout = TimeSpan.FromSeconds(7);
                //in case you want to debug this unit test
                if (Debugger.IsAttached)
                {
                    timeout = TimeSpan.FromSeconds(300);
                }
                return timeout;
            }
        }
    }
}