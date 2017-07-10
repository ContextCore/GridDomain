using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using Xunit;

namespace GridDomain.Tests.Unit.Serialization
{
    public class Aggregate_can_be_used_for_snapshotting_by_defaut
    {
        private TestAggregate _restoredAggregate;
        private TestAggregate _aggregate;

        private class TestAggregate : Aggregate
        {
            public TestAggregate(int value, Guid id) : base(id)
            {
                Produce(new TestAggregateCreatedEvent(id, value));
            }

            public int Value { get; private set; }

            private void Apply(TestAggregateCreatedEvent e)
            {
                Id = e.SourceId;
                Value = e.Value;
            }

            internal class TestAggregateCreatedEvent : DomainEvent
            {
                public TestAggregateCreatedEvent(Guid sourceId, int value) : base(sourceId)
                {
                    Value = value;
                }

                public int Value { get; }
            }
        }

        [Fact]
        public void Aggregate_by_default_can_be_saved_as_IMemento_for_snapshot()
        {
            _aggregate = new TestAggregate(1, Guid.NewGuid());
            var snapshot = ((IAggregate) _aggregate).GetSnapshot();
            var factory = new AggregateFactory();
            _restoredAggregate = factory.Build<TestAggregate>(_aggregate.Id, snapshot);

            // Restored_aggregate_is_not_null()
            Assert.NotNull(_restoredAggregate);
            // Ids_are_equal()
            Assert.Equal(_aggregate.Id, _restoredAggregate.Id);
            //Restored_aggregate_uncommitted_events_are_empty()
            Assert.Empty(((IAggregate) _restoredAggregate).GetUncommittedEvents());
            //Restored_aggregate_state_is_equal_to_origin()
            Assert.Equal(_aggregate.Value, _restoredAggregate.Value);
        }
    }
}