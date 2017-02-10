using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Adapters;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.EventsUpgrade;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
    public class Future_events_upgraded_by_events_adapter : NodeTestKit
    {
       
        [Fact]
        public async Task Future_event_is_upgraded_by_event_adapter()
        {
            var saveOldEventCommand = new ChangeBalanceInFuture(1, Guid.NewGuid(), BusinessDateTime.Now.AddSeconds(2), true);

            await Node.Prepare(saveOldEventCommand)
                      .Expect<FutureEventScheduledEvent>(e => e.Event.SourceId == saveOldEventCommand.AggregateId)
                      .Execute();

            await Node.NewWaiter()
                      .Expect<BalanceChangedEvent_V1>()
                      .Create();
        }

   

        class EventAdaptersFixture : BalanceFixture
        {
            public EventAdaptersFixture()
            {
                Add(new CustomContainerConfiguration(
                    c => c.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig()),
                    c => c.RegisterInstance<IPersistentChildsRecycleConfiguration>(
                            new PersistentChildsRecycleConfiguration(TimeSpan.FromMilliseconds(100),
                                TimeSpan.FromMilliseconds(50)))));
            }

            protected override void OnNodeCreated()
            {
                Node.EventsAdaptersCatalog.Register(new BalanceChanged_eventdapter1());
            }

            class BalanceChanged_eventdapter1 : DomainEventAdapter<BalanceChangedEvent_V0, BalanceChangedEvent_V1>
            {
                public override IEnumerable<BalanceChangedEvent_V1> ConvertEvent(BalanceChangedEvent_V0 evt)
                {
                    yield return new BalanceChangedEvent_V1(evt.AmplifiedAmountChange, evt.SourceId);
                }
            }
        }

        public Future_events_upgraded_by_events_adapter(ITestOutputHelper output) : base(output, new EventAdaptersFixture()) {}
    }
}