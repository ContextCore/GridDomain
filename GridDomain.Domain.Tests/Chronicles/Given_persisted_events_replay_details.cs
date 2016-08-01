using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Chronicles
{
    [TestFixture]
    public class Given_persisted_events_replay_details : SampleDomainCommandExecutionTests
    {
        [Test]
        public void When_replaying_not_existing_id_should_throw_an_exception()
        {
            // var aggregateId = Guid.NewGuid();
            // var events = new DomainEvent[]
            // {
            //     new SampleAggregateCreatedEvent("123", aggregateId),
            //     new SampleAggregateChangedEvent("234", aggregateId)
            // };

        }

        public Given_persisted_events_replay_details(bool inMemory) : base(inMemory)
        {
        }
    }
}