using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.DependencyInjection.Infrastructure;
using GridDomain.Tests.XUnit.FutureEvents;
using GridDomain.Tests.XUnit.FutureEvents.Infrastructure;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.FutureDomainEvents
{
    public class FutureEventsTest_Persistent_restart
    {
        private readonly FutureEventsFixture _fixture;

        public FutureEventsTest_Persistent_restart(ITestOutputHelper helper)
        {
            _fixture = new FutureEventsFixture(helper) {InMemory = false};
        }

        [Fact]
        public async Task It_fires_after_node_restart()
        {
            var node = await _fixture.CreateNode();
            var cmd = new ScheduleEventInFutureCommand(DateTime.Now.AddSeconds(3), Guid.NewGuid(), "test value");

            await node.Prepare(cmd)
                      .Expect<FutureEventScheduledEvent>(e => e.Event.SourceId == cmd.AggregateId)
                      .Execute();

            //to finish job persist
            await Task.Delay(300);

            await node.Stop();
            _fixture.System = null;

            await node.Start();

            var res = await node.NewWaiter(TimeSpan.FromSeconds(10))
                                .Expect<FutureEventOccuredEvent>(e => e.SourceId == cmd.AggregateId)
                                .Create();

            var evt = res.Message<FutureEventOccuredEvent>();

            Assert.True(evt.CreatedTime - cmd.RaiseTime <= TimeSpan.FromSeconds(2));
        }
    }
}