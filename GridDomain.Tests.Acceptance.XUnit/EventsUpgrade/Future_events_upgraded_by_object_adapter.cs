using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events;
using GridDomain.Tests.XUnit.FutureEvents;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
    public class Future_events_upgraded_by_object_adapter : NodeTestKit
    {
        [Fact]
        public async Task Future_event_is_upgraded_by_json_adapter()
        {
            var saveOldEventCommand = new ChangeBalanceInFuture(1, Guid.NewGuid(), BusinessDateTime.Now.AddSeconds(2), true);

            await Node.Prepare(saveOldEventCommand)
                      .Expect<FutureEventScheduledEvent>()
                      .Execute();

            await Node.NewWaiter().Expect<BalanceChangedEvent_V1>().Create();
        }

        class FutureEventsAdapterFixture : FutureEventsFixture
        {
            public FutureEventsAdapterFixture()
            {
                Add(new CustomContainerConfiguration(
                    c => c.RegisterAggregate<BalanceAggregate, BalanceAggregatesCommandHandler>(),
                    c =>
                        c.RegisterInstance<IPersistentChildsRecycleConfiguration>(
                            new PersistentChildsRecycleConfiguration(TimeSpan.FromMilliseconds(100),
                                TimeSpan.FromMilliseconds(50)))));
                Add(new BalanceRouteMap());
            }

            protected override void OnNodeCreated()
            {
                base.OnNodeCreated();
                Node.EventsAdaptersCatalog.Register(new BalanceChanged_objectAdapter1());
            }
        }

      

        public Future_events_upgraded_by_object_adapter(ITestOutputHelper output) : base(output, new FutureEventsAdapterFixture()) {}
    }
}
