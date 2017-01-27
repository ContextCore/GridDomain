using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;

using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.SyncProjection
{
    [TestFixture]
    public class SynchronizedProjectionBuildersTests : InMemorySampleDomainTests
    {
        private Dictionary<Guid, DomainEvent[]> _eventsPerAggregate;

        protected override TimeSpan DefaultTimeout => TimeSpan.FromMinutes(1);

        [OneTimeSetUp]
        public async Task When_execute_many_commands_for_create_and_update()
        {
            var createCommands = Enumerable.Range(0, 10).Select(r => new CreateSampleAggregateCommand(101, Guid.NewGuid())).ToArray();
            var aggregateIds = createCommands.Select(c => c.AggregateId).ToArray();
            var updateCommands = Enumerable.Range(0, 40).Select(r => new ChangeSampleAggregateCommand(102, aggregateIds.RandomElement())).ToArray();

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
                messageWaiter.And<SampleAggregateChangedEvent>(e => e.SourceId == id && e.Value == c.Parameter.ToString());
            }

           var task = messageWaiter.Create();

            GridNode.Execute(createCommands);
            GridNode.Execute(updateCommands);

            _eventsPerAggregate = (await task).All
                .Cast<IMessageMetadataEnvelop>()
                .Select(m => m.Message as IHaveProcessingHistory)
                .GroupBy(e => e.SourceId)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.History.ElapsedTicksFromAppStart)
                        .Cast<DomainEvent>().ToArray());
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
        public void All_events_related_to_one_aggregate_should_be_processed_with_same_projection_group()
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
