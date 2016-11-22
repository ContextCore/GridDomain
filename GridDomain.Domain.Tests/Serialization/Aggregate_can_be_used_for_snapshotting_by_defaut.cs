using System;
using CommonDomain;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.FutureEvents;
using NUnit.Framework;

namespace GridDomain.Tests.Serialization
{
    [TestFixture]
    class Aggregate_can_be_used_for_snapshotting_by_defaut
    {
        private TestAggregate _restoredAggregate;
        private TestAggregate _aggregate;

        class TestAggregate : Aggregate
        {
            public int Value { get; private set; }

            public TestAggregate(int value, Guid id):base(id)
            {
               RaiseEvent(new TestAggregateCreatedEvent(id, value));
            }

            private void Apply(TestAggregateCreatedEvent e)
            {
                Id = e.SourceId;
                Value = e.Value;
            }

            internal class TestAggregateCreatedEvent : DomainEvent
            {
                public int Value { get; }

                public TestAggregateCreatedEvent(Guid id, int value) : base(id)
                {
                    Value = value;
                }
            }
        }

        [OneTimeSetUp]
        public void Aggregate_by_default_can_be_saved_as_IMemento_for_snapshot()
        {
            _aggregate = new TestAggregate(1,Guid.NewGuid());
            var snapshot = ((IAggregate)_aggregate).GetSnapshot();
            var factory = new AggregateFactory();
            _restoredAggregate = factory.Build<TestAggregate>(_aggregate.Id, snapshot);
        }

        [Test]
        public void Ids_are_equal()
        {
            Assert.AreEqual(_aggregate.Id, _restoredAggregate.Id);
        }

        [Test]
        public void Restored_aggregate_is_not_null()
        {
            Assert.NotNull(_restoredAggregate);
        }

        [Test]
        public void Restored_aggregate_uncommitted_events_are_empty()
        {
            CollectionAssert.IsEmpty(((IAggregate)_restoredAggregate).GetUncommittedEvents());
        }

        [Test]
        public void Restored_aggregate_state_is_equal_to_origin()
        {
            Assert.AreEqual(_aggregate.Value, _restoredAggregate.Value);
        }
    }

}