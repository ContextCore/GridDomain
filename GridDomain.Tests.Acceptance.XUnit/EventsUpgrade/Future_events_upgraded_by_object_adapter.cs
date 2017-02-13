using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.EventsUpgrade;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events;
using GridDomain.Tests.XUnit.FutureEvents;
using Microsoft.Practices.Unity;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
    public static class TestFixtureExtensions
    {
        public static void InitFastRecycle(this NodeTestFixture fixture, TimeSpan? clearPeriod = null, TimeSpan? maxInactiveTime = null)
        {
            fixture.Add(new CustomContainerConfiguration(
            c => c.RegisterInstance<IPersistentChildsRecycleConfiguration>(
                    new PersistentChildsRecycleConfiguration(clearPeriod ?? TimeSpan.FromMilliseconds(100),
                        maxInactiveTime ?? TimeSpan.FromMilliseconds(50)))));
        }
    }
    public class Future_events_upgraded_by_object_adapter : NodeTestKit
    {
        [Fact]
        public async Task Future_event_is_upgraded_by_json_adapter()
        {
            var cmd = new ChangeBalanceInFuture(1, 
                                                Guid.NewGuid(), 
                                                BusinessDateTime.Now.AddSeconds(2),
                                                false);

            var res = await Node.Prepare(cmd)
                                .Expect<BalanceChangedEvent_V1>()
                                .Execute();

            //event should be modified by json object adapter, chaning its Amount
            Assert.Equal(101, res.Message<BalanceChangedEvent_V1>().AmountChange);
        }

        class FutureEventsAdapterFixture : BalanceFixture
        {
            public FutureEventsAdapterFixture()
            {
               this.InitFastRecycle();
            }

            protected override void OnNodeCreated()
            {
                Node.EventsAdaptersCatalog.Register(new IncreaseBy100Adapter());
            }
            class IncreaseBy100Adapter : ObjectAdapter<BalanceChangedEvent_V1, BalanceChangedEvent_V1>
            {
                public override BalanceChangedEvent_V1 Convert(BalanceChangedEvent_V1 evt)
                {
                    return new BalanceChangedEvent_V1(evt.AmountChange + 100, evt.SourceId, evt.CreatedTime, evt.SagaId);
                }
            }
        }

        public Future_events_upgraded_by_object_adapter(ITestOutputHelper output)
            : base(output, new FutureEventsAdapterFixture() {InMemory = false}) {}
    }
}