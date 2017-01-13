using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    [TestFixture]
    class GridNode_upgrade_events_by_json_adapters_when_loading_aggregate : SampleDomainCommandExecutionTests
    {
        protected override bool ClearDataOnStart { get; } = true;

        public GridNode_upgrade_events_by_json_adapters_when_loading_aggregate():base(false)
        {
          
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

        protected override void OnNodeCreated()
        {
            GridNode.EventsAdaptersCatalog.Register(new InIncreaseByInstanceAdapter());
            GridNode.EventsAdaptersCatalog.Register(new NullAdapter());
        }

        [Test]
        public async Task Then_domain_events_should_be_upgraded_by_json_custom_adapter()
        {
            var cmd = new CreateSampleAggregateCommand(1, Guid.NewGuid());

            await GridNode.Prepare(cmd)
                          .Expect<SampleAggregateCreatedEvent>()
                          .Execute();

            var aggregate = LoadAggregate<SampleAggregate>(cmd.AggregateId);

            Assert.AreEqual("101", aggregate.Value);
        }
    }
}