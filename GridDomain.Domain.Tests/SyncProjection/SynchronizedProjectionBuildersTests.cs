using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.SyncProjection
{
    [TestFixture]
    public class SynchronizedProjectionBuildersTests : InMemorySampleDomainTests
    {
        private Dictionary<Guid, DomainEvent[]> _eventsPerAggregate;

        protected override TimeSpan Timeout => TimeSpan.FromMinutes(1);
        

        [OneTimeSetUp]
        public async Task When_execute_many_commands_for_create_and_update()
        {
            var createCommands = Enumerable.Range(0, 10).Select(r => new CreateSampleAggregateCommand(101, Guid.NewGuid())).ToArray();
            var aggregateIds = createCommands.Select(c => c.AggregateId).ToArray();
            var updateCommands = Enumerable.Range(0, 10).Select(r => new ChangeSampleAggregateCommand(102, aggregateIds.RandomElement())).ToArray();

            var aggregateId = createCommands.First().AggregateId;

            var messageWaiter = GridNode.NewWaiter().Expect<SampleAggregateCreatedEvent>(e => e.SourceId == aggregateId);
            foreach (var c in createCommands.Skip(1))
            {
                var id = c.AggregateId;
                messageWaiter.And<SampleAggregateCreatedEvent>(e => e.SourceId == id);
            }

            foreach (var c in updateCommands)
            {
                var id = c.AggregateId;
                messageWaiter.And<SampleAggregateChangedEvent>(e => e.SourceId == id);
            }

           var task = messageWaiter.Create();

            GridNode.Execute(createCommands);
            GridNode.Execute(updateCommands);

            _eventsPerAggregate = (await task).All
                .Cast<IMessageMetadataEnvelop>()
                .Select(m => m.Message as DomainEvent)
                .GroupBy(e => e.SourceId)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.CreatedTime).ToArray());
        }

        [Test]
        public void All_events_related_to_one_aggregate_processed_time_should_be_only_increasing()
        {
            AllEventsForOneAggregate_should_be_ordered_by(e => e.History.ElapsedTicksFromAppStart);
        }

        [Test]
        public void All_events_related_to_one_aggregate_processed_number_should_be_only_increasing()
        {
            AllEventsForOneAggregate_should_be_ordered_by(e => e.History.SequenceNumber);
        }

        [Test]
        public void All_events_related_to_one_aggregate_processor_should_be_the_same()
        {
            AllEventsForOneAggregate_should_be_ordered_by(e => e.History.ProjectionGroupHashCode);
        }

        private void AllEventsForOneAggregate_should_be_ordered_by(Func<IHaveProcessingHistory, long> valueSelector)
        {
         

            foreach (var eventsForOneAggregate in _eventsPerAggregate)
            {
                CheckOrderedValues(eventsForOneAggregate.Value.Cast<IHaveProcessingHistory>(),
                                   valueSelector);
            }
        }

        private void CheckOrderedValues<TElem>(IEnumerable<TElem> elements, Func<TElem, long> valueSelector)
        {
            long prevEventTime = 0;
            foreach (var element in elements)
            {
                var currentValue = valueSelector(element);
                Assert.GreaterOrEqual(currentValue, prevEventTime);
                prevEventTime = currentValue;
            }
        }

    }
}
