using System;
using System.Threading.Tasks;
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
using GridDomain.Tests.FutureEvents;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{

    class BalanceChanged_objectAdapter1 : ObjectAdapter<BalanceChangedEvent_V0, BalanceChangedEvent_V1>
    {
        public override BalanceChangedEvent_V1 Convert(BalanceChangedEvent_V0 evt)
        {
            return new BalanceChangedEvent_V1(evt.AmplifiedAmountChange, evt.SourceId);
        }
    }

    internal class PersistentChildsRecycleConfiguration : IPersistentChildsRecycleConfiguration
    {
        public PersistentChildsRecycleConfiguration(TimeSpan childClearPeriod, TimeSpan childMaxInactiveTime)
        {
            ChildClearPeriod = childClearPeriod;
            ChildMaxInactiveTime = childMaxInactiveTime;
        }

        public TimeSpan ChildClearPeriod { get; }
        public TimeSpan ChildMaxInactiveTime { get; }
    }

    [TestFixture]
    public class Future_events_upgraded_by_object_adapter : FutureEventsTest
    { 
        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(c => c.RegisterAggregate<BalanceAggregate, BalanceAggregatesCommandHandler>(),
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

        protected override void OnNodeCreated()
        {
            base.OnNodeCreated();
            GridNode.EventsAdaptersCatalog.Register(new BalanceChanged_objectAdapter1());
        }

        [Test]
        public async Task Future_event_is_upgraded_by_json_adapter()
        {
            var saveOldEventCommand = new ChangeBalanceInFuture(1,Guid.NewGuid(),BusinessDateTime.Now.AddSeconds(2),true);

            await GridNode.PrepareCommand(saveOldEventCommand)
                          .Expect<FutureEventScheduledEvent>()
                          .Execute(Timeout);

            await GridNode.NewWaiter(Timeout).Expect<BalanceChangedEvent_V1>().Create();
        }

        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(10);
    }
}
