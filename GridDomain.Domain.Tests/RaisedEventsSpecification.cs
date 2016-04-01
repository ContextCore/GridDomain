using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Domain.Tests
{
    [TestFixture]
    public abstract class RaisedEventsSpecification<TAggregate> where TAggregate : IAggregate
    {
        protected TAggregate Aggregate;

        protected abstract TAggregate CreateAggregate { get; }

        protected virtual IEnumerable<DomainEvent> ExpectedEvents
        {
            get { yield break; }
        }

        protected abstract void When();

        [SetUp]
        public virtual void Init()
        {
            Aggregate = CreateAggregate;
            When();
        }

        [Test]
        public void VerifyRaisedEvents()
        {
            var expected = ExpectedEvents;
            var aggregate = ((IAggregate) Aggregate);
            var published = aggregate.GetUncommittedEvents().OfType<DomainEvent>();
            EventsExtensions.CompareEvents(expected.ToArray(), published.ToArray());
        }

        protected void ClearUncommittedEvents()
        {
            ((IAggregate)Aggregate).ClearUncommittedEvents();
        }
    }
}