using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Serialization;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers;
using GridDomain.Tests.XUnit;
using Microsoft.Practices.Unity;
using Moq;
using Xunit;
using Xunit.Abstractions;
using GridDomain.Scheduling.Quartz.Logging;
using Quartz;
using IScheduler = Quartz.IScheduler;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling
{
    public class Spec: NodeTestKit
    {
        private const string Name = "test";
        private const string Group = "test";
        private IActorRef _scheduler;
        private IScheduler _quartzScheduler;
        private IUnityContainer _container;

        public Spec(ITestOutputHelper output) : this(output, new SchedulerFixture())
        {
            ResultHolder.Clear();
        }

        public Spec(ITestOutputHelper output, SchedulerFixture fixture) : base(output, fixture)
        {
            _container = fixture.Node.Container;
            _quartzScheduler = fixture.QuartzScheduer;
            _scheduler = fixture.SchedulerActor;
        }

        public class SchedulerFixture : NodeTestFixture
        {
            public IScheduler QuartzScheduer { get; private set; }
            public IActorRef SchedulerActor { get; private set; }

            public SchedulerFixture()
            {
                Add(new SchedulerContainerConfiguration());
                Add(new TestRouter());
            }

            protected override void OnNodeStarted()
            {
               QuartzScheduer = Node.Container.Resolve<Quartz.IScheduler>();
               QuartzScheduer.Clear();
               SchedulerActor = Node.System.ActorOf(Node.System.DI().Props<SchedulingActor>());
            }

            private class SchedulerContainerConfiguration : IContainerConfiguration
            {
                public void Register(IUnityContainer container)
                {
                    container.RegisterInstance<IRetrySettings>(new InMemoryRetrySettings(4, TimeSpan.Zero));
                    container.RegisterInstance(new Mock<LoggingSchedulerListener>().Object);
                    container.RegisterAggregate<TestAggregate, TestAggregateCommandHandler>();
                    container.RegisterType<IPersistentChildsRecycleConfiguration, DefaultPersistentChildsRecycleConfiguration>();
                }
            }
        }


        private ExtendedExecutionOptions CreateOptions(double seconds, TimeSpan? timeout=null,Guid? id=null, string checkField = null, int? retryCount = null, TimeSpan? repeatInterval = null)
        {
            return new ExtendedExecutionOptions(BusinessDateTime.UtcNow.AddSeconds(seconds),
                                                typeof(ScheduledCommandSuccessfullyProcessed),
                                                id ?? Guid.Empty,
                                                checkField,
                                                timeout ?? Fixture.DefaultTimeout);
        }

        
        [Fact]
        public void When_system_resolves_scheduler_Then_single_instance_is_returned_in_all_cases()
        {
            var sched1 = _container.Resolve<IScheduler>();
            var sched2 = _container.Resolve<IScheduler>();
            var sched3 = _container.Resolve<IScheduler>();
            Assert.True(sched1 == sched2 && sched2 == sched3);
        }

        [Fact]
        public void When_system_shuts_down_the_scheduler_Then_the_next_resolve_will_return_another_instance()
        {
            var sched1 = _container.Resolve<IScheduler>();
            sched1.Shutdown(false);
            var sched2 = _container.Resolve<IScheduler>();
            Assert.True(sched1 != sched2);
        }


        class CallbackJobListener : IJobListener
        {
            public TaskCompletionSource<Tuple<IJobExecutionContext, JobExecutionException>> TaskFinish
                 = new TaskCompletionSource<Tuple<IJobExecutionContext, JobExecutionException>>();

            public void JobToBeExecuted(IJobExecutionContext context)
            {
            }

            public void JobExecutionVetoed(IJobExecutionContext context)
            {
            }

            public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
            {
                TaskFinish.SetResult(new Tuple<IJobExecutionContext, JobExecutionException>(context,jobException));
            }
            
            public string Name { get; } = "Callback Listener";
        }

        [Fact]
        public async Task When_job_fails_it_retries_several_times()
        {
            var cmd = new PlanFailuresCommand(Guid.NewGuid(), 3);
            await Node.Prepare(cmd)
                      .Expect<ScheduledCommandProcessingFailuresPlanned>()
                      .Execute();

            var failCommand = new FailIfPlannedCommand(cmd.AggregateId, TimeSpan.FromMilliseconds(10));

            var options = CreateOptions(0.5,
                                        TimeSpan.FromSeconds(0.5),
                                        cmd.AggregateId,
                                        nameof(DomainEvent.SourceId));

            await _scheduler.Ask<Scheduled>(new ScheduleCommand(failCommand, new ScheduleKey(Guid.Empty, Name, Group), options));
            await Node.NewWaiter(TimeSpan.FromSeconds(5)).Expect<JobFailed>().Create();
            //should fail first 3 times and execute on forth
            Console.WriteLine("Received first failure");
            await Node.NewWaiter(TimeSpan.FromSeconds(5)).Expect<JobFailed>().Create();
            Console.WriteLine("Received second failure");
            await Node.NewWaiter(TimeSpan.FromSeconds(5)).Expect<JobFailed>().Create();
            Console.WriteLine("Received third failure");
            await Node.NewWaiter(TimeSpan.FromSeconds(5)).Expect<JobCompleted>().Create();
            Console.WriteLine("Received success");
        }

        [Fact]
        public void When_there_are_several_scheduled_jobs_System_executes_all_of_them()
        {
            var tasks = new[] { 0.5, 0.6, 0.7, 0.8, 1 };

            foreach (var task in tasks)
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task, Fixture.DefaultTimeout)));
            }

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.AssertInTime(() => ResultHolder.Contains(taskIds));
        }

        [Fact]
        public void When_client_tries_to_add_two_task_with_same_id_Then_only_one_gets_executed()
        {
            var testMessage = new SuccessCommand(Name);
            _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(0.5, Fixture.DefaultTimeout)));
            _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(1, Fixture.DefaultTimeout)));

            Throttle.AssertInTime(() => Assert.True(ResultHolder.Count == 1), minTimeout: TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void When_some_of_scheduled_jobs_fail_System_still_executes_others()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var failTasks = new[] { 1.0, 2.0 };

            foreach (var task in successTasks)
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testCommand = new SuccessCommand(text);
                _scheduler.Tell(new ScheduleCommand(testCommand, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task, Fixture.DefaultTimeout)));
            }
            foreach (var failTask in failTasks)
            {
                var text = failTask.ToString(CultureInfo.InvariantCulture);
                var failTaskCommand = new FailCommand();
                _scheduler.Tell(new ScheduleCommand(failTaskCommand, new ScheduleKey(Guid.Empty, text, text), CreateOptions(failTask, Fixture.DefaultTimeout)));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var failTaskIds = failTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.AssertInTime(() =>
            {
                ResultHolder.Contains(successTaskIds);
                Assert.True(failTaskIds.All(x => ResultHolder.Get(x) == null));
            }, minTimeout: TimeSpan.FromSeconds(3));
        }

        [Fact]
        public void When_tasks_get_deleted_after_scheduling_System_will_not_execute_them()
        {
            var successTasks = new[] { 0.5, 1.5, 2.5 };
            var tasksToRemove = new[] { 1.0, 2.0 };

            foreach (var task in successTasks.Concat(tasksToRemove))
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task, Fixture.DefaultTimeout)));
            }

            var successTaskIds = successTasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            var tasksToRemoveTaskIds = tasksToRemove.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            foreach (var taskId in tasksToRemoveTaskIds)
            {
                _scheduler.Tell(new Unschedule(new ScheduleKey(Guid.Empty, taskId, taskId)));
            }

            Throttle.AssertInTime(() =>
            {
                ResultHolder.Contains(successTaskIds);
                Assert.True(tasksToRemoveTaskIds.All(x => ResultHolder.Get(x) == null));
            }, minTimeout: TimeSpan.FromSeconds(4));
        }

        [Fact]
        public void When_scheduler_is_restarted_Then_scheduled_jobs_still_get_executed()
        {
            var tasks = new[] { 0.5, 1, 1.5, 2, 2.5 };

            foreach (var task in tasks)
            {
                var text = task.ToString(CultureInfo.InvariantCulture);
                var testMessage = new SuccessCommand(text);
                _scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, text, text), CreateOptions(task, Fixture.DefaultTimeout)));
            }

            _quartzScheduler.Shutdown(false);
            _quartzScheduler = _container.Resolve<IScheduler>();

            var taskIds = tasks.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();

            Throttle.AssertInTime(() => ResultHolder.Contains(taskIds));
        }
    }
}