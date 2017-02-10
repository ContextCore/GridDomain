using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
   
    class GridNode_upgrade_events_by_json_adapters_when_loading_aggregate : NodeTestKit
    {

        protected GridNode_upgrade_events_by_json_adapters_when_loading_aggregate(ITestOutputHelper output)
            : base(output, new EventsUpgradeFixture())
        {
        }

        class EventsUpgradeFixture : SampleDomainFixture
        {
            protected override void OnNodeCreated()
            {
                Node.EventsAdaptersCatalog.Register(new InIncreaseByInstanceAdapter());
                Node.EventsAdaptersCatalog.Register(new NullAdapter());
            }

            class InIncreaseByInstanceAdapter : ObjectAdapter<string, string>
            {
                public override string Convert(string value)
                {
                    return value + "01";
                }
            }


            class NullAdapter : ObjectAdapter<SampleAggregateCreatedEvent, SampleAggregateCreatedEvent>
            {
                public override SampleAggregateCreatedEvent Convert(SampleAggregateCreatedEvent value)
                {
                    return new SampleAggregateCreatedEvent(value.Value + "01",
                                                           value.SourceId,
                                                           value.CreatedTime,
                                                           value.SagaId);
                }
            }
        }


        [Fact]
        public async Task Then_domain_events_should_be_upgraded_by_json_custom_adapter()
        {
            var cmd = new CreateSampleAggregateCommand(1, Guid.NewGuid());

            await Node.Prepare(cmd)
                      .Expect<SampleAggregateCreatedEvent>()
                      .Execute();

            var aggregate = await Node.LoadAggregate<SampleAggregate>(cmd.AggregateId);

            Assert.Equal("101", aggregate.Value);
        }
    }
}