using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz.Configuration;
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
        //TODO: fix in future
        [Fact(Skip = "some problems on appveyor")]
        public async Task It_fires_after_node_restart()
        {

            var node = await new FutureEventsFixture(_testOutputHelper, new PersistedQuartzConfig()).UseSqlPersistence().CreateNode();
            var cmd = new ScheduleEventInFutureCommand(BusinessDateTime.UtcNow.AddSeconds(5), Guid.NewGuid().ToString(), "test value");

            await node.Execute(cmd);
            await node.Stop();

            node = await new FutureEventsFixture(_testOutputHelper, new PersistedQuartzConfig(),false).UseSqlPersistence(false).CreateNode();
            _testOutputHelper.WriteLine("starting waiter creation");
            var res = await node.NewWaiter(TimeSpan.FromSeconds(30)) 
                                .Expect<FutureEventOccuredEvent>(e => e.SourceId == cmd.AggregateId)
                                .Create();

            var evt = res.Message<FutureEventOccuredEvent>();

            Assert.True(evt.CreatedTime - cmd.RaiseTime <= TimeSpan.FromSeconds(2));
        }
    }
}