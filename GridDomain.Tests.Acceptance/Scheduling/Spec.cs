using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Tests.Acceptance.Persistence;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;
using GridDomain.Tests.Configuration;
using Microsoft.Practices.Unity;
using Moq;
using NUnit.Framework;
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
        private IUnityContainer _container;

        public Spec() : base(new AutoTestAkkaConfiguration().ToStandAloneSystemConfig())
        {

        }

        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var system = ActorSystemFactory.CreateActorSystem(akkaConf);
            _container = Register(system);
            system.AddDependencyResolver(new UnityDependencyResolver(_container, system));
            var router = new TestRouter();
            return new GridDomainNode(_container, router, TransportMode.Standalone, system);
        }

        private IUnityContainer Register(ActorSystem system)
        {
            var container = Container.CreateChildScope();
            Node.CompositionRoot.Init(container, system, new AutoTestLocalDbConfiguration(), TransportMode.Standalone);
            container.RegisterInstance(new Mock<ILoggingSchedulerListener>().Object);
            container.RegisterType<AggregateActor<TestAggregate>>();
            container.RegisterType<AggregateHubActor<TestAggregate>>();
            container.RegisterType<ICommandAggregateLocator<TestAggregate>, TestAggregateCommandHandler>();
            container.RegisterType<IAggregateCommandsHandler<TestAggregate>, TestAggregateCommandHandler>();
            return container;
        }

        [SetUp]
        public void SetUp()
        {
            LogManager.SetLoggerFactory(new TestLoggerFactory());
            CreateScheduler();
            _scheduler = GridNode.System.ActorOf(GridNode.System.DI().Props<SchedulingActor>());
            _quartzScheduler.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            TestLogger.Clear();
            ResultHolder.Clear();
            _quartzScheduler.Shutdown(true);
        }

        private void CreateScheduler()
        {
            _quartzScheduler = _container.Resolve<IScheduler>();
        }

        [Test]
        public void When_two_commands_with_same_success_event_are_published_Then_first_successfully_handled_command_doesnt_change_second_commands_saga_state()
        {
            var firstCommand = new TimeoutCommand("timeout", TimeSpan.FromMilliseconds(300));
            var secondCommand = new TimeoutCommand("timeout", TimeSpan.FromSeconds(3));
            var firstKey = Guid.NewGuid();
            var secondKey = Guid.NewGuid();
            _scheduler.Ask<Scheduled>(new Schedule(firstCommand, new ScheduleKey(firstKey, Id, Group), CreateOptions(0))).Wait(Timeout);
            _scheduler.Ask<Scheduled>(new Schedule(secondCommand, new ScheduleKey(secondKey, Id + Id, Group), CreateOptions(0))).Wait(Timeout);
            WaitFor<ScheduledCommandSuccessfullyProcessed>();
            Thread.Sleep(500);
            var firstSagaState = LoadSagaState<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>(firstKey);
            var secondSaga = LoadSagaState<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>(secondKey);
            Assert.True(firstSagaState.MachineState == ScheduledCommandProcessingSaga.States.SuccessfullyProcessed && secondSaga.MachineState == ScheduledCommandProcessingSaga.States.MessageSent);
        }



        [Test]
        public void When_two_commands_of_the_same_type_are_published_Then_first_failed_command_doesnt_change_second_commands_saga_state()
        {
            var firstCommand = new FailCommand(TimeSpan.FromMilliseconds(300));
            var secondCommand = new FailCommand(TimeSpan.FromSeconds(3));
            var firstKey = Guid.NewGuid();
            var secondKey = Guid.NewGuid();
            _scheduler.Ask<Scheduled>(new Schedule(firstCommand, new ScheduleKey(firstKey, Id, Group), CreateOptions(0))).Wait(Timeout);
            _scheduler.Ask<Scheduled>(new Schedule(secondCommand, new ScheduleKey(secondKey, Id + Id, Group), CreateOptions(0))).Wait(Timeout);
            WaitFor<CommandFault<FailCommand>>();
            Thread.Sleep(500);
            var firstSagaState = LoadSagaState<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>(firstKey);
            var secondSaga = LoadSagaState<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>(secondKey);
            Assert.True(firstSagaState.MachineState == ScheduledCommandProcessingSaga.States.ProcessingFailure && secondSaga.MachineState == ScheduledCommandProcessingSaga.States.MessageSent);
        }

        [Test]
        public void When_a_message_published_Then_saga_receives_it()
        {
            Thread.Sleep(1000);
            var testCommand = new SuccessCommand(Id);
            _scheduler.Ask<Scheduled>(new Schedule(testCommand, new ScheduleKey(Guid.Empty, Id, Group), CreateOptions(1))).Wait(Timeout);
            WaitFor<SagaCreatedEvent<ScheduledCommandProcessingSaga.States>>();
        }

        private ExecutionOptions CreateOptions(double seconds)
        {
            return new ExecutionOptions<ScheduledCommandSuccessfullyProcessed>(DateTime.UtcNow.AddSeconds(seconds), Timeout);
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
        public void When_job_is_added_Then_it_gets_executed()
        {
            var successCommand = new SuccessCommand(Id);
            _scheduler.Ask<Scheduled>(new Schedule(successCommand, new ScheduleKey(Guid.Empty, Id, Group), CreateOptions(0.5))).Wait(Timeout);

            WaitFor<ScheduledCommandSuccessfullyProcessed>();
            Throttle.Assert(() => Assert.True(ResultHolder.Contains(successCommand.Text)), maxTimeout: Timeout);
        }

        [Test]
        public void When_scheduler_is_restarted_during_job_execution_Then_on_next_start_job_is_not_fired_again()
        {
            var timeoutCommand = new TimeoutCommand(Id, TimeSpan.FromSeconds(2));
            _scheduler.Ask<Scheduled>(new Schedule(timeoutCommand, new ScheduleKey(Guid.Empty, Id, Group), CreateOptions(0.5))).Wait(Timeout);
            WaitFor<CompleteJob>();
            _quartzScheduler.Shutdown(false);
            CreateScheduler();
            WaitFor<ScheduledCommandSuccessfullyProcessed>();
            Assert.True(ResultHolder.Count == 1 && ResultHolder.Contains(Id));
        }

        [Test]
        public void When_processing_actor_throws_Then_scheduler_receives_failure_response()
        {
            var testMessage = new FailCommand();
            var id = Guid.NewGuid();
            _scheduler.Tell(new Schedule(testMessage, new ScheduleKey(id, Id, Group), CreateOptions(0.5)));
            //TODO::VZ:: to really test system I need a way to check that scheduling saga received the message
            //TODO::VZ:: get saga from persistence
            WaitFor<CommandFault<FailCommand>>();
            var sagaState = LoadSagaState<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>(id);
            Assert.True(sagaState.MachineState == ScheduledCommandProcessingSaga.States.ProcessingFailure);
        }

        [Test]
        public void When_there_are_several_scheduled_jobs_System_executes_all_of_them()
        {
            var tasks = new[] { 0.5, 0.6, 0.7, 0.8, 1 };

            foreach (var task in tasks)
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new Schedule(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() => ResultHolder.Contains(taskIds));
        }

        [Test]
        public void When_client_tries_to_add_two_task_with_same_id_Then_only_one_gets_executed()
        {
            var testMessage = new SuccessCommand(Id);
            _scheduler.Tell(new Schedule(testMessage, new ScheduleKey(Guid.Empty, Id, Group), CreateOptions(0.5)));
            _scheduler.Tell(new Schedule(testMessage, new ScheduleKey(Guid.Empty, Id, Group), CreateOptions(1)));

            Throttle.Assert(() => Assert.True(ResultHolder.Count == 1), minTimeout: TimeSpan.FromSeconds(2));
        }

        [Test]
        public void When_some_of_scheduled_jobs_fail_System_still_executes_others()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var failTasks = new[] { 1.0, 2.0 };

            foreach (var task in successTasks)
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testCommand = new SuccessCommand(text);
                _scheduler.Tell(new Schedule(testCommand, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }
            foreach (var failTask in failTasks)
            {
                var text = failTask.ToString(CultureInfo.InvariantCulture);
                var failTaskCommand = new FailCommand();
                _scheduler.Tell(new Schedule(failTaskCommand, new ScheduleKey(Guid.Empty, text, text), CreateOptions(failTask)));
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
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new Schedule(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var tasksToRemoveTaskIds = tasksToRemove.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            foreach (var taskId in tasksToRemoveTaskIds)
            {
                _scheduler.Tell(new Unschedule(new ScheduleKey(Guid.Empty, taskId, taskId)));
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
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new Schedule(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
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