using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Domain.Tests
{
    public abstract class EventsSpecification
    {
        protected InMemoryEventRepository Repository { get; private set; }

        [Test]
        public void When()
        {
            Repository = new InMemoryEventRepository(new AggregateFactory());
            var expected = Expect().ToList();
            var published = Repository.ProducedEvents;
            EventsExtensions.CompareEvents(expected.ToArray(), published.ToArray());
        }

        protected virtual IEnumerable<DomainEvent> Given()
        {
            yield break;
        }

        protected abstract IEnumerable<DomainEvent> Expect();
    }
}