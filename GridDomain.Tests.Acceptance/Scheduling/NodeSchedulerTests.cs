using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Acceptance.Scheduling.TestHelpers;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Scheduling
{

    public class NodeSchedulerTests : NodeTestKit
    {
        public class SchedulerFixture : NodeTestFixture
        {
            public SchedulerFixture(ITestOutputHelper output):base(output)
            {
                Add(new TestSchedulingAggregateDomainConfiguration());
                this.EnableScheduling(new InMemoryRetrySettings(4, TimeSpan.Zero));
            }
        }

        protected NodeSchedulerTests(NodeTestFixture fixture) : base(fixture){}
        public NodeSchedulerTests(ITestOutputHelper output) : this(new SchedulerFixture(output))
        {
            ResultHolder.Clear();
        }

        private const string Name = "test";
        private const string Group = "test1";

        private IActorRef Scheduler => Node.System.GetSchedulingExtension()
                                           .SchedulingActor;

        private ExecutionOptions CreateOptions(double seconds,
                                               TimeSpan? timeout = null,
                                               string id = null,
                                               string checkField = null,
                                               int? retryCount = null,
                                               TimeSpan? repeatInterval = null)
        {
            return new ExecutionOptions(BusinessDateTime.UtcNow.AddSeconds(seconds),
                                        timeout);
        }

        [Fact]
        public async Task When_client_tries_to_add_two_task_with_same_id_Then_only_one_gets_executed()
        {
            var testMessage = new SuccessCommand("yes!");
            Scheduler.Tell(new ScheduleCommandExecution(testMessage, new ScheduleKey(Name, Group), CreateOptions(0.5)));
            Scheduler.Tell(new ScheduleCommandExecution(testMessage, new ScheduleKey(Name, Group), CreateOptions(1)));

            AwaitAssert(() => Assert.Equal(1,ResultHolder.Count),TimeSpan.FromSeconds(10),TimeSpan.FromSeconds(3));
        }
    }
}