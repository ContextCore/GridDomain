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
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
    public class Future_events_class_upgraded_by_object_adapter : NodeTestKit
    {
        public Future_events_class_upgraded_by_object_adapter(ITestOutputHelper output) : base(output,
                                                             new EventAdaptersFixture() { InMemory = false })
        { }

        [Fact]
        public async Task Future_event_is_upgraded_by_event_adapter()
        {
            var saveOldEventCommand = new ChangeBalanceInFuture(1, Guid.NewGuid(), BusinessDateTime.Now.AddSeconds(2), true);

            await Node.Prepare(saveOldEventCommand)
                      .Expect<BalanceChangedEvent_V1>()
                      .Execute();
        }

        class EventAdaptersFixture : BalanceFixture
        {
            public EventAdaptersFixture()
            {
                this.InitFastRecycle();
                InMemory = false;
                LogLevel = LogEventLevel.Debug;
                OnNodeCreatedEvent += (sender, args) =>
                                       Node.EventsAdaptersCatalog.Register(new BalanceChanged_eventdapter1());
            }

            class BalanceChanged_eventdapter1: ObjectAdapter<BalanceChangedEvent_V0, BalanceChangedEvent_V1>
            {
                public override BalanceChangedEvent_V1 Convert(BalanceChangedEvent_V0 evt)
                {
                    return new BalanceChangedEvent_V1(evt.AmplifiedAmountChange, evt.SourceId);
                }
            }
        }


    }
}