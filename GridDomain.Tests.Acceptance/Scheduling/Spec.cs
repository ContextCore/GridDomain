using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Logging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;
using GridDomain.Tests.Framework;
using Microsoft.Practices.Unity;
using Moq;
using NMoneys;
using NUnit.Framework;
using Wire;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    [TestFixture]
    public class Spec : ExtendedNodeCommandTest
    {
        private const string Name = "test";
        private const string Group = "test";
        private IActorRef _scheduler;
        private IScheduler _quartzScheduler;
        private IUnityContainer _container;

        public Spec() : base(false)
        {

        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new SchedulerContainerConfiguration();
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new TestRouter();
        }

        private class SchedulerContainerConfiguration : IContainerConfiguration
        {
            public void Register(IUnityContainer container)
            {
                container.RegisterInstance(new Mock<ILoggingSchedulerListener>().Object);
                //container.RegisterType<AggregateActor<TestAggregate>>();
                //container.RegisterType<AggregateHubActor<TestAggregate>>();
                //container.RegisterType<ICommandAggregateLocator<TestAggregate>, TestAggregateCommandHandler>();
                //container.RegisterType<IAggregateCommandsHandler<TestAggregate>, TestAggregateCommandHandler>();
                container.RegisterAggregate<TestAggregate, TestAggregateCommandHandler>();
                //container.RegisterType<ISagaFactory<TestSaga, TestSagaState>, TestSagaFactory>();
                //container.RegisterType<ISagaFactory<TestSaga, TestSagaStartMessage>, TestSagaFactory>();
                //container.RegisterType<ISagaFactory<TestSaga, Guid>, TestSagaFactory>();
                container.RegisterStateSaga<TestSaga, TestSagaState, TestSagaStartMessage, TestSagaFactory>();
            }
        }

        [SetUp]
        public void SetUp()
        {
            TypesForScalarDescruptionHolder.Add(typeof(Money));
            LogManager.SetLoggerFactory(new DefaultLoggerFactory());
            
            DateTimeStrategyHolder.Current = new DefaultDateTimeStrategy();
            _container = GridNode.Container;
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
        public void LogTest()
        {
            LogManager.GetLogger().Error(new InvalidOperationException("ohshitwaddap"), "message {placeholder}", new { Money = new Money(123, CurrencyIsoCode.RUB) });
        }

        [Test]
        public void When_domain_event_that_should_start_a_saga_is_scheduled_Then_saga_gets_created()
        {
            var sagaId = Guid.NewGuid();
            var testEvent = new TestSagaStartMessage(sagaId, DateTimeFacade.UtcNow, sagaId);
            _scheduler.Ask<Scheduled>(new ScheduleMessage(testEvent, new ScheduleKey(Guid.Empty, Name, Group), DateTime.UtcNow.AddSeconds(0.3)));
            WaitFor<SagaCreatedEvent<TestSaga.TestStates>>();
            var sagaState = LoadSagaState<TestSaga, TestSagaState, TestSagaStartMessage>(sagaId);
            Assert.True(sagaState.MachineState == TestSaga.TestStates.GotStartEvent);
        }

        [Test]
        public void When_domain_event_for_a_started_saga_is_scheduled_Then_saga_receives_it()
        {
            var sagaId = Guid.NewGuid();
            var startEvent = new TestSagaStartMessage(sagaId, DateTimeFacade.UtcNow, sagaId);
            _scheduler.Ask<Scheduled>(new ScheduleMessage(startEvent, new ScheduleKey(Guid.Empty, Name, Group), DateTime.UtcNow.AddSeconds(0.3)));
            WaitFor<SagaCreatedEvent<TestSaga.TestStates>>();

            var secondEvent = new TestEvent(sagaId);
            _scheduler.Ask<Scheduled>(new ScheduleMessage(secondEvent, new ScheduleKey(Guid.Empty, Name, Group), DateTime.UtcNow.AddSeconds(0.3)));
            WaitFor<SagaTransitionEvent<TestSaga.TestStates, TestSaga.Transitions>>();
            var sagaState = LoadSagaState<TestSaga, TestSagaState, TestSagaStartMessage>(sagaId);
            Assert.True(sagaState.MachineState == TestSaga.TestStates.GotSecondEvent);
        }

        [Test]
        public void When_two_commands_with_same_success_event_are_published_Then_first_successfully_handled_command_doesnt_change_second_commands_saga_state()
        {
            var firstCommand = new TimeoutCommand("timeout", TimeSpan.FromMilliseconds(300));
            var secondCommand = new TimeoutCommand("timeout", TimeSpan.FromSeconds(3));
            var firstKey = Guid.NewGuid();
            var secondKey = Guid.NewGuid();
            _scheduler.Ask<Scheduled>(new ScheduleCommand(firstCommand, new ScheduleKey(firstKey, Name, Group), CreateOptions(0))).Wait(Timeout);
            _scheduler.Ask<Scheduled>(new ScheduleCommand(secondCommand, new ScheduleKey(secondKey, Name + Name, Group), CreateOptions(0))).Wait(Timeout);
            WaitFor<SagaTransitionEvent<ScheduledCommandProcessingSaga.States, ScheduledCommandProcessingSaga.Transitions>>();
            Thread.Sleep(1000);
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
            _scheduler.Ask<Scheduled>(new ScheduleCommand(firstCommand, new ScheduleKey(firstKey, Name, Group), CreateOptions(0))).Wait(Timeout);
            _scheduler.Ask<Scheduled>(new ScheduleCommand(secondCommand, new ScheduleKey(secondKey, Name + Name, Group), CreateOptions(0))).Wait(Timeout);
            Thread.Sleep(2000);
            var firstSagaState = LoadSagaState<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>(firstKey);
            var secondSaga = LoadSagaState<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState, ScheduledCommandProcessingStarted>(secondKey);
            Assert.True(firstSagaState.MachineState == ScheduledCommandProcessingSaga.States.ProcessingFailure && secondSaga.MachineState == ScheduledCommandProcessingSaga.States.MessageSent);
        }

        [Test]
        public void Serializer_can_serialize_and_deserialize_polymorphic_types()
        {
            var withType = new ExecutionOptions<ScheduledCommandSuccessfullyProcessed>(DateTime.MaxValue);
            var serializer = new Serializer();
            var stream = new MemoryStream();
            serializer.Serialize(withType, stream);
            var bytes = stream.ToArray();
            var deserialized = serializer.Deserialize<ExecutionOptions>(new MemoryStream(bytes));
            Assert.True(deserialized.SuccessEventType == withType.SuccessEventType);
        }


        [Test]
        public void When_a_message_published_Then_saga_receives_it()
        {
            var testCommand = new SuccessCommand(Name);
            _scheduler.Ask<Scheduled>(new ScheduleCommand(testCommand, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(1))).Wait(Timeout);
            WaitFor<SagaCreatedEvent<ScheduledCommandProcessingSaga.States>>();
        }

        private ExecutionOptions CreateOptions(double seconds)
        {
            return new ExecutionOptions<ScheduledCommandSuccessfullyProcessed>(DateTimeFacade.UtcNow.AddSeconds(seconds), Timeout);
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
            var successCommand = new SuccessCommand(Name);
            _scheduler.Ask<Scheduled>(new ScheduleCommand(successCommand, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(0.5))).Wait(Timeout);

            WaitFor<ScheduledCommandSuccessfullyProcessed>();
            Throttle.Assert(() => Assert.True(ResultHolder.Contains(successCommand.Text)), maxTimeout: Timeout);
        }

        [Test]
        public void When_scheduler_is_restarted_during_job_execution_Then_on_next_start_job_is_not_fired_again()
        {
            var timeoutCommand = new TimeoutCommand(Name, TimeSpan.FromSeconds(2));
            _scheduler.Ask<Scheduled>(new ScheduleCommand(timeoutCommand, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(0.5))).Wait(Timeout);
            WaitFor<CompleteJob>();
            _quartzScheduler.Shutdown(false);
            CreateScheduler();
            WaitFor<ScheduledCommandSuccessfullyProcessed>();
            Assert.True(ResultHolder.Count == 1 && ResultHolder.Contains(Name));
        }

        [Test]
        public void When_processing_actor_throws_Then_scheduler_receives_failure_response()
        {
            var testMessage = new FailCommand();
            var id = Guid.NewGuid();
            _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(id, Name, Group), CreateOptions(0.5)));
            //TODO::VZ:: to really test system I need a way to check that scheduling saga received the message
            //TODO::VZ:: get saga from persistence
            WaitFor<CommandFault<FailCommand>>(false);
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
                _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.Assert(() => ResultHolder.Contains(taskIds));
        }

        [Test]
        public void When_client_tries_to_add_two_task_with_same_id_Then_only_one_gets_executed()
        {
            var testMessage = new SuccessCommand(Name);
            _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(0.5)));
            _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(1)));

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
                _scheduler.Tell(new ScheduleCommand(testCommand, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }
            foreach (var failTask in failTasks)
            {
                var text = failTask.ToString(CultureInfo.InvariantCulture);
                var failTaskCommand = new FailCommand();
                _scheduler.Tell(new ScheduleCommand(failTaskCommand, new ScheduleKey(Guid.Empty, text, text), CreateOptions(failTask)));
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
                _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
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
                _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task)));
            }

            _quartzScheduler.Shutdown(false);
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