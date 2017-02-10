using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
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
    public class Future_events_upgraded_by_object_adapter : NodeTestKit
    {
        [Fact]
        public async Task Future_event_is_upgraded_by_json_adapter()
        {
            var res = await Node.Prepare(new ChangeBalanceInFuture(1, 
                                                         Guid.NewGuid(), 
                                                         BusinessDateTime.Now.AddSeconds(0.5),
                                                         false))
                                .Expect<BalanceChangedEvent_V1>()
                                .Execute();

            Assert.Equal(101, res.Message<BalanceChangedEvent_V1>().AmountChange);
        }

        class FutureEventsAdapterFixture : BalanceFixture
        {
            public FutureEventsAdapterFixture()
            {
                Add(
                    new CustomContainerConfiguration(
                        c =>
                            c.RegisterInstance<IPersistentChildsRecycleConfiguration>(
                                new PersistentChildsRecycleConfiguration(TimeSpan.FromMilliseconds(100),
                                    TimeSpan.FromMilliseconds(50)))));

                LogLevel = LogEventLevel.Information;
            }

            protected override void OnNodeCreated()
            {
                Node.EventsAdaptersCatalog.Register(new IncreaseBy100Adapter());
            }
            class IncreaseBy100Adapter : ObjectAdapter<decimal, decimal>
            {
                public override decimal Convert(decimal evt)
                {
                    return evt + 100;
                }
            }
        }

        public Future_events_upgraded_by_object_adapter(ITestOutputHelper output)
            : base(output, new FutureEventsAdapterFixture()) {}
    }
}