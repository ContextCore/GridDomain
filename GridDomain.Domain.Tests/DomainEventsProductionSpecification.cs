using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Domain.Tests
{
    [TestFixture]
    public abstract class DomainEventsProductionSpecification<TAggregate> where TAggregate : IAggregate
    {
        [SetUp]
        public void Init()
        {
            Aggregate = CreateAggregate();

            var domainEvents = Given().ToList();
            if (domainEvents.Any())
            {
                foreach (var e in domainEvents)
                    Aggregate.ApplyEvent(e);

                ClearUncommittedEvents();
            }

            When();
        }

        protected TAggregate Aggregate;

        protected virtual TAggregate CreateAggregate()
        {
            return new AggregateFactory().Build<TAggregate>(Guid.NewGuid());
        }

        protected abstract IEnumerable<DomainEvent> ExpectedEvents();

        protected virtual void When()
        {
        }

        protected void ClearUncommittedEvents()
        {
            ((IAggregate) Aggregate).ClearUncommittedEvents();
        }

        protected virtual IEnumerable<DomainEvent> Given()
        {
            return Enumerable.Empty<DomainEvent>();
        }

        protected void VerifyRaisedEvents()
        {
            var expected = ExpectedEvents();
            var aggregate = (IAggregate) Aggregate;
            var published = aggregate.GetUncommittedEvents().OfType<DomainEvent>();
            EventsExtensions.CompareEvents(expected.ToArray(), published.ToArray());
        }
    }
}