using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.EventsUpgrade.Domain;
using GridDomain.Tests.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.EventsUpgrade.Domain.Events;
using GridDomain.Tests.Framework;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    [TestFixture]
    [Ignore("Under development")]
    public class Future_events_upgraded_by_object_adapter : ExtendedNodeCommandTest
    {

        class BalanceChanged_objectAdapter1 : ObjectAdapter<BalanceChangedEvent_V0, BalanceChangedEvent_V1>
        {
            public override BalanceChangedEvent_V1 Convert(BalanceChangedEvent_V0 evt)
            {
                return new BalanceChangedEvent_V1(evt.AmplifiedAmountChange, evt.SourceId);
            }
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(c => c.RegisterAggregate<BalanceAggregate, BalanceAggregatesCommandHandler>(),
                                                    c => c.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig()),
                                                    c => c.RegisterInstance<IPersistentChildsRecycleConfiguration>(
                                                        new PersistentChildsRecycleConfiguration(
                                                            TimeSpan.FromMilliseconds(100), 
                                                            TimeSpan.FromMilliseconds(50))));
        }


        protected override IMessageRouteMap CreateMap()
        {
            return new BalanceRouteMap();
        }

        public Future_events_upgraded_by_object_adapter():base(false)
        {
            
        }
        [Test]
        public void Future_event_is_upgraded_by_json_adapter()
        {
            GridNode.EventsAdaptersCatalog.Register(new BalanceChanged_objectAdapter1());

            var saveOldEventCommand = new ChangeBalanceInFuture(1,Guid.NewGuid(),BusinessDateTime.Now.AddSeconds(2),true);

            GridNode.NewCommandWaiter(Timeout)
                    .Expect<FutureEventScheduledEvent>(e => e.Event.SourceId == saveOldEventCommand.AggregateId)
                    .Create()
                    .Execute(saveOldEventCommand)
                    .Wait();

            GridNode.NewWaiter(Timeout).Expect<BalanceChangedEvent_V1>().Create().Wait();
        }

        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(10);
    }
}
