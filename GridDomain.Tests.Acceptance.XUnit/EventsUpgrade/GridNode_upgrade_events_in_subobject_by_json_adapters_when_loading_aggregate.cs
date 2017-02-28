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
    public class GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate : NodeTestKit
    {
        public GridNode_upgrade_events_in_subobject_by_json_adapters_when_loading_aggregate(ITestOutputHelper output)
            : base(output, new EventsUpgradeFixture {InMemory = false}) {}

        private class EventsUpgradeFixture : SampleDomainFixture
        {
            public EventsUpgradeFixture()
            {
                OnNodeCreatedEvent += (sender, args) => Node.EventsAdaptersCatalog.Register(new String01Adapter());
            }

            private class String01Adapter : ObjectAdapter<string, string>
            {
                public override string Convert(string value)
                {
                    return value + "01";
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