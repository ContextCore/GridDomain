using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Abstractions;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    class TestSchedulingAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(DefaultAggregateDependencyFactory.New(new TestAggregateCommandHandler()));
        }
    }

    public class NodeSchedulerTests : NodeTestKit
    {
        public class SchedulerFixture : NodeTestFixture
        {
            public SchedulerFixture()
            {
               Add(new TestSchedulingAggregateDomainConfiguration());
               this.ClearSheduledJobs();
            }

            protected override NodeSettings CreateNodeSettings()
            {
                var settings = base.CreateNodeSettings();
                settings.QuartzJobRetrySettings = new InMemoryRetrySettings(4, TimeSpan.Zero);
                return settings;
            }
        }

        public NodeSchedulerTests(ITestOutputHelper output) : base(output, new SchedulerFixture())
        {
            ResultHolder.Clear();
            _schedulerActor = new Lazy<IActorRef>(() => Node.ResolveActor(nameof(SchedulingActor)).Result);
        }

        private const string Name = "test";
        private const string Group = "test";
        private readonly Lazy<IActorRef> _schedulerActor;
        private IActorRef Scheduler => _schedulerActor.Value;

        private ExecutionOptions CreateOptions(double seconds,
                                                       TimeSpan? timeout = null,
                                                       Guid? id = null,
                                                       string checkField = null,
                                                       int? retryCount = null,
                                                       TimeSpan? repeatInterval = null)
        {
            return new ExecutionOptions(BusinessDateTime.UtcNow.AddSeconds(seconds),
                                                typeof(ScheduledCommandSuccessfullyProcessed),
                                                id ?? Guid.Empty,
                                                checkField,
                                                timeout);
        }

        [Fact]
        public async Task When_client_tries_to_add_two_task_with_same_id_Then_only_one_gets_executed()
        {
            var testMessage = new SuccessCommand("yes!");
            Scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(0.5)));
            Scheduler.Tell(new ScheduleCommand(testMessage, new ScheduleKey(Guid.Empty, Name, Group), CreateOptions(1)));

            await Task.Delay(2000);
            Assert.True(ResultHolder.Count == 1);
        }

        [Fact]
        public void When_system_resolves_scheduler_Then_single_instance_is_returned_in_all_cases()
        {
            var sched1 = Node.Container.Resolve<IScheduler>();
            var sched2 = Node.Container.Resolve<IScheduler>();
            var sched3 = Node.Container.Resolve<IScheduler>();
            Assert.True(sched1 == sched2 && sched2 == sched3);
        }
    }
}