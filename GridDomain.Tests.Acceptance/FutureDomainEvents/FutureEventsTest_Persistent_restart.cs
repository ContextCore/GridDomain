using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.Acceptance.Snapshots;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.FutureEvents;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.FutureDomainEvents
{
    public class FutureEventsTest_Persistent_restart
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FutureEventsTest_Persistent_restart(ITestOutputHelper helper)
        {
            _testOutputHelper = helper;
        }

        [Fact]
        public async Task It_fires_after_node_restart()
        {

            var node = await new FutureEventsFixture(_testOutputHelper).UseSqlPersistence().CreateNode();
            var cmd = new ScheduleEventInFutureCommand(DateTime.UtcNow.AddSeconds(3), Guid.NewGuid(), "test value");

            await node.Prepare(cmd)
                      .Expect<CommandExecutionScheduled>()
                      .Execute(TimeSpan.FromSeconds(10));

            await node.Stop();

            node = await new FutureEventsFixture(_testOutputHelper).UseSqlPersistence(false).CreateNode();
            var res = await node.NewWaiter(TimeSpan.FromSeconds(10))
                                .Expect<FutureEventOccuredEvent>(e => e.SourceId == cmd.AggregateId)
                                .Create();

            var evt = res.Message<FutureEventOccuredEvent>();

            Assert.True(evt.CreatedTime - cmd.RaiseTime <= TimeSpan.FromSeconds(2));
        }
    }
}