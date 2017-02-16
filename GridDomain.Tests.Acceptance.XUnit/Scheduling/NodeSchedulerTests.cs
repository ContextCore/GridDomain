using System;
using System.Diagnostics;
using System.IO;
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
    public class NodeSchedulerTests: NodeTestKit
    {
        class SchedulerContainerConfiguration : IContainerConfiguration
        {
            public void Register(IUnityContainer container)
            {
                container.RegisterInstance<IRetrySettings>(new InMemoryRetrySettings(4, TimeSpan.Zero));
                container.RegisterInstance(new Mock<LoggingSchedulerListener>().Object);
                container.RegisterAggregate<TestAggregate, TestAggregateCommandHandler>();
                container.RegisterType<IPersistentChildsRecycleConfiguration, DefaultPersistentChildsRecycleConfiguration>();
            }
        }

        private const string Name = "test";
        private const string Group = "test";
        private readonly IActorRef _scheduler;
      

        public NodeSchedulerTests(ITestOutputHelper output) : this(output, new SchedulerFixture())
        {
            ResultHolder.Clear();
        }

        private NodeSchedulerTests(ITestOutputHelper output, SchedulerFixture fixture) : base(output, fixture)
        {
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

          
        }

        private ExtendedExecutionOptions CreateOptions(double seconds, TimeSpan? timeout=null,Guid? id=null, string checkField = null, int? retryCount = null, TimeSpan? repeatInterval = null)
        {
            return new ExtendedExecutionOptions(BusinessDateTime.UtcNow.AddSeconds(seconds),
                                                typeof(ScheduledCommandSuccessfullyProcessed),
                                                id ?? Guid.Empty,
                                                checkField,
                                                timeout);
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
        public void When_system_resolves_scheduler_Then_single_instance_is_returned_in_all_cases()
        {
            var sched1 = Node.Container.Resolve<IScheduler>();
            var sched2 = Node.Container.Resolve<IScheduler>();
            var sched3 = Node.Container.Resolve<IScheduler>();
            Assert.True(sched1 == sched2 && sched2 == sched3);
        }

        [Fact]
        public void When_system_shuts_down_the_scheduler_Then_the_next_resolve_will_return_another_instance()
        {
            var sched1 = Node.Container.Resolve<IScheduler>();
            sched1.Shutdown(false);
            var sched2 = Node.Container.Resolve<IScheduler>();
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

    }
}