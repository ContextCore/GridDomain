using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CommonDomain;
using GridDomain.EventSourcing;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Framework
{
    public class AggregateTest<TAggregate> where TAggregate : IAggregate
    {
        protected TAggregate Aggregate;

        protected virtual IEnumerable<DomainEvent> Given()
        {
            yield break;
        }

        protected static Fixture Data { get; } = new Fixture();
        private readonly AggregateFactory aggregateFactory = new AggregateFactory();
        protected DomainEvent[] GivenEvents { get; private set; }

        protected virtual void Init()
        {
            Aggregate = (TAggregate) aggregateFactory.Build(typeof(TAggregate), Guid.NewGuid(), null);
            GivenEvents = Given().ToArray();
            Aggregate.ApplyEvents(GivenEvents);
        }
    }
}
