﻿using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.EventsUpgrade;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
    public class Future_events_upgraded_by_object_adapter : NodeTestKit
    {
        public Future_events_upgraded_by_object_adapter(ITestOutputHelper output)
            : base(output, new FutureEventsAdapterFixture {InMemory = false}) {}

        private class FutureEventsAdapterFixture : BalanceFixture
        {
            public FutureEventsAdapterFixture()
            {
                this.InitFastRecycle();
                OnNodeCreatedEvent += (sender, args) => Node.EventsAdaptersCatalog.Register(new IncreaseBy100Adapter());
            }

            private class IncreaseBy100Adapter : ObjectAdapter<BalanceChangedEvent_V1, BalanceChangedEvent_V1>
            {
                public override BalanceChangedEvent_V1 Convert(BalanceChangedEvent_V1 evt)
                {
                    return new BalanceChangedEvent_V1(evt.AmountChange + 100, evt.SourceId, evt.CreatedTime, evt.SagaId);
                }
            }
        }

        [Fact]
        public async Task Future_event_is_upgraded_by_json_adapter()
        {
            var cmd = new ChangeBalanceInFuture(1, Guid.NewGuid(), BusinessDateTime.Now.AddSeconds(2), false);

            var res = await Node.Prepare(cmd)
                                .Expect<BalanceChangedEvent_V1>()
                                .Execute();

            //event should be modified by json object adapter, changing its Amount
            Assert.Equal(101,
                res.Message<BalanceChangedEvent_V1>()
                   .AmountChange);
        }
    }
}