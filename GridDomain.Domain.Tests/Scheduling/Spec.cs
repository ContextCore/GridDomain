using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.TestKit.NUnit;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.WebUI;
using GridDomain.Tests.Scheduling.TestHelpers;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Tests.Scheduling
{
    [TestFixture]
    public class Spec : TestKit
    {
        private const string Id = "test";
        private const string Group = "test";
        private IActorRef _scheduler;
        private IScheduler _quartzScheduler;
        private Mock<IQuartzLogger> _quartzLogger;
        private UnityContainer _container;
        private IActorSubscriber _subsriber;

        private UnityContainer Register()
        {
            var container = Container.Current;
            container.RegisterType<QuartzJob>();
            container.RegisterType<ISchedulerFactory, SchedulerFactory>();
            container.RegisterType<IScheduler>(new InjectionFactory(x => x.Resolve<ISchedulerFactory>().GetScheduler()));
            var loggingJobListener = new Mock<ILoggingJobListener>();
            loggingJobListener.Setup(x => x.Name).Returns("testListener");
            //container.RegisterInstance(loggingJobListener.Object);
            container.RegisterType<ILoggingJobListener, LoggingJobListener>();
            //container.RegisterInstance(new Mock<ILoggingSchedulerListener>().Object);
            container.RegisterType<ILoggingSchedulerListener, LoggingSchedulerListener>();
            container.RegisterType<IQuartzConfig, QuartzConfig>();
            container.RegisterType<ActorSystem>(new InjectionFactory(x => Sys));
            var transport = new AkkaEventBusTransport(Sys);
            container.RegisterInstance<IPublisher>(transport);
            container.RegisterInstance<IActorSubscriber>(transport);
            container.RegisterType<IWebUiConfig, WebUiConfig>();
            container.RegisterType<IWebUiWrapper, WebUiWrapper>();
            _quartzLogger = new Mock<IQuartzLogger>();
            container.RegisterInstance(_quartzLogger.Object);
            QuartzLoggerFactory.SetLoggerFactory(() => _quartzLogger.Object);
            container.RegisterType<SchedulingActor>();
            container.RegisterType<SuccessfulTestMessageHandler>();
            container.RegisterType<FailingTestRequestHandler>();
            container.RegisterType(typeof(TestRequestHandler<>));
            return container;
        }

        [SetUp]
        public void SetUp()
        {
            _container = Register();
            Sys.AddDependencyResolver(new UnityDependencyResolver(_container, Sys));
            CreateScheduler();
            _subsriber = _container.Resolve<IActorSubscriber>();

            _scheduler = Sys.ActorOf(Sys.DI().Props<SchedulingActor>());
            _quartzScheduler.Clear();
        }

        private void CreateScheduler()
        {
            _quartzScheduler = _container.Resolve<IScheduler>();
        }

        [Test]
        public void When_system_resolves_scheduler_Then_single_instance_is_returned_in_all_cases()
        {
            var sched1 = _container.Resolve<IScheduler>();
            var sched2 = _container.Resolve<IScheduler>();
            var sched3 = _container.Resolve<IScheduler>();
            Assert.True(sched2 == sched1 && sched2 == sched3);
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
                var testMessage = new TestMessage("web", "web");
                var resultHolder = new ResultHolder();
                var testActor = ActorOfAsTestActorRef<SuccessfulTestMessageHandler>(Props.Create(() => new SuccessfulTestMessageHandler(resultHolder)));
                _subsriber.Subscribe(testMessage.GetType(), testActor);
                _scheduler.Ask<Scheduled>(new Schedule(testMessage, runAt, Timeout)).Wait(Timeout);
                _scheduler.Ask<Scheduled>(new Schedule(new TestMessage(Id, Group), DateTime.UtcNow.AddSeconds(10), Timeout)).Wait(Timeout);
                _scheduler.Ask<Scheduled>(new Schedule(new TestMessage(Id + Id, Group), DateTime.UtcNow.AddSeconds(15), Timeout)).Wait(Timeout);
                Throttle.Assert(() => Assert.True(resultHolder.Contains("web")), maxTimeout: TimeSpan.FromHours(1));
            }
        }

        [Test]
        public void When_job_is_added_Then_it_gets_executed()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var testMessage = new TestMessage(Id, Group);
            var resultHolder = new ResultHolder();
            var testActor = ActorOfAsTestActorRef<SuccessfulTestMessageHandler>(Props.Create(() => new SuccessfulTestMessageHandler(resultHolder)));
            _subsriber.Subscribe(testMessage.GetType(), testActor);
            _scheduler.Ask<Scheduled>(new Schedule(testMessage, runAt, Timeout)).Wait(Timeout);
            Throttle.Assert(() => Assert.True(resultHolder.Contains(testMessage.TaskId)), maxTimeout: Timeout);
        }

        [Test]
        public void When_scheduler_is_restarted_during_job_execution_Then_on_next_start_job_is_fired_again()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var testMessage = new TestMessage(Id, Group);
            var resultHolder = new ResultHolder();
            Action<TestMessage> handler = msg =>
            {
                resultHolder.Add(DateTime.UtcNow.Ticks.ToString(), "1");
                Thread.Sleep(2000);
            };
            var testActor = ActorOfAsTestActorRef<TestRequestHandler<TestMessage>>(Props.Create(() => new TestRequestHandler<TestMessage>(handler)));
            _subsriber.Subscribe(testMessage.GetType(), testActor);
            _scheduler.Ask<Scheduled>(new Schedule(testMessage, runAt, Timeout)).Wait(Timeout);
            Thread.Sleep(500);
            _quartzScheduler.Shutdown(false);
            Thread.Sleep(1000);
            CreateScheduler();
                                                                                                                //it takes a lot of time to scheduler to actually fire job second time
            Throttle.Assert(() => Assert.True(resultHolder.Count == 2), minTimeout: TimeSpan.FromSeconds(2.5), maxTimeout: TimeSpan.FromSeconds(20));
        }

        [Test]
        public void When_processing_actor_throws_Then_scheduler_receives_failure_response()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var testMessage = new FailTaskMessage(Id, Group);
            var testActor = ActorOfAsTestActorRef<FailingTestRequestHandler>();
            _subsriber.Subscribe(testMessage.GetType(), testActor);
            _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            Throttle.Verify(_quartzLogger, x => x.LogFailure(testMessage.TaskId, It.IsAny<Exception>()), maxTimeout: Timeout);
        }



        [Test]
        public void When_there_are_several_scheduled_jobs_System_executes_all_of_them()
        {
            var tasks = new[] { 0.5, 1, 1.5, 2, 2.5 };

            var resultHolder = new ResultHolder();
            var testActor = ActorOfAsTestActorRef<SuccessfulTestMessageHandler>(Props.Create(() => new SuccessfulTestMessageHandler(resultHolder)));
            _subsriber.Subscribe(typeof(TestMessage), testActor);

            foreach (var task in tasks)
            {
                var testMessage = new TestMessage(task.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            }

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() => resultHolder.Contains(taskIds));
        }

        [Test]
        public void When_client_tries_to_add_two_task_with_same_id_Then_only_one_gets_executed()
        {
            var runAt = DateTime.UtcNow.AddSeconds(0.5);
            var secondRunAt = DateTime.UtcNow.AddSeconds(1);
            var taskId = "taskId";
            var result = new List<string>();

            Action<TestMessage> handler = request => result.Add(request.TaskId);

            var testMessage = new TestMessage(taskId, Group);
            var testActor = ActorOfAsTestActorRef<TestRequestHandler<TestMessage>>(Props.Create(() => new TestRequestHandler<TestMessage>(handler)));
            _subsriber.Subscribe(typeof(TestMessage), testActor);
            _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            _scheduler.Tell(new Schedule(testMessage, secondRunAt, Timeout));

            Throttle.Assert(() => Assert.True(result.Count == 1), minTimeout: TimeSpan.FromSeconds(2));
        }

        [Test]
        public void When_some_of_scheduled_jobs_fail_System_still_executes_others()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var failTasks = new[] { 1.0, 2.0 };

            var resultHolder = new ResultHolder();
            var testActor = ActorOfAsTestActorRef<SuccessfulTestMessageHandler>(Props.Create(() => new SuccessfulTestMessageHandler(resultHolder)));
            _subsriber.Subscribe(typeof(TestMessage), testActor);

            foreach (var task in successTasks)
            {
                var testMessage = new TestMessage(task.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            }
            foreach (var failTask in failTasks)
            {
                var failRequest = new FailTaskMessage(failTask.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(failTask);
                _scheduler.Tell(new Schedule(failRequest, runAt, Timeout));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var failTaskIds = failTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() =>
            {
                resultHolder.Contains(successTaskIds);
                Assert.True(failTaskIds.All(x => resultHolder.Get(x) == null));
            }, minTimeout: TimeSpan.FromSeconds(3));
        }

        [Test]
        public void When_tasks_get_deleted_after_scheduling_System_will_not_execute_them()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var tasksToRemove = new[] { 1.0, 2.0 };

            var resultHolder = new ResultHolder();
            var testActor = ActorOfAsTestActorRef<SuccessfulTestMessageHandler>(Props.Create(() => new SuccessfulTestMessageHandler(resultHolder)));
            _subsriber.Subscribe(typeof(TestMessage), testActor);

            foreach (var task in successTasks.Concat(tasksToRemove))
            {
                var testMessage = new TestMessage(task.ToString(CultureInfo.InvariantCulture), Group);
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
                resultHolder.Contains(successTaskIds);
                Assert.True(tasksToRemoveTaskIds.All(x => resultHolder.Get(x) == null));
            }, minTimeout: TimeSpan.FromSeconds(4));
        }

        [Test]
        public void When_scheduler_is_restarted_Then_scheduled_jobs_still_get_executed()
        {
            var tasks = new[] { 0.5, 1, 1.5, 2, 2.5 };

            var resultHolder = new ResultHolder();
            var testActor = ActorOfAsTestActorRef<SuccessfulTestMessageHandler>(Props.Create(() => new SuccessfulTestMessageHandler(resultHolder)));
            _subsriber.Subscribe(typeof(TestMessage), testActor);

            foreach (var task in tasks)
            {
                var testMessage = new TestMessage(task.ToString(CultureInfo.InvariantCulture), Group);
                var runAt = DateTime.UtcNow.AddSeconds(task);
                _scheduler.Tell(new Schedule(testMessage, runAt, Timeout));
            }

            _quartzScheduler.Shutdown(false);
            Thread.Sleep(2000);
            CreateScheduler();

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() => resultHolder.Contains(taskIds));
        }



        private static TimeSpan Timeout
        {
            get
            {
                //default timeout
                var timeout = TimeSpan.FromSeconds(5);
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