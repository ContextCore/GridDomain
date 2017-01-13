using System;
using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using GridDomain.CQRS;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    [TestFixture]
    [Ignore("Under development")]
    public class Future_events_upgraded_by_events_adapter : ExtendedNodeCommandTest
    {
        class BalanceChanged_eventdapter1 : DomainEventAdapter<BalanceChangedEvent_V0, BalanceChangedEvent_V1>
        {
            public override IEnumerable<BalanceChangedEvent_V1> ConvertEvent(BalanceChangedEvent_V0 evt)
            {
                yield return new BalanceChangedEvent_V1(evt.AmplifiedAmountChange, evt.SourceId);
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

        public Future_events_upgraded_by_events_adapter() : base(false)
        {

        }

        protected override void OnNodeCreated()
        {
            GridNode.EventsAdaptersCatalog.Register(new BalanceChanged_eventdapter1());
        }

        [Test]
        public void Future_event_is_upgraded_by_event_adapter()
        {

            var saveOldEventCommand = new ChangeBalanceInFuture(1, Guid.NewGuid(), BusinessDateTime.Now.AddSeconds(2), true);

            GridNode.Prepare(saveOldEventCommand)
                    .Expect<FutureEventScheduledEvent>(e => e.Event.SourceId == saveOldEventCommand.AggregateId)
                    .Execute(Timeout)
                    .Wait();

            GridNode.NewWaiter(Timeout).Expect<BalanceChangedEvent_V1>().Create().Wait();
        }

        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(10);
    }
}