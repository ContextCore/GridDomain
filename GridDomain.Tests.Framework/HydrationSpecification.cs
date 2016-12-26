using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Framework
{
    [TestFixture]
    public class HydrationSpecification<TAggregate> where TAggregate : IAggregate
    {
        public HydrationSpecification()
        {
            Aggregate = (TAggregate) aggregateFactory.Build(typeof (TAggregate), Guid.NewGuid(), null);
        }
        public void ApplyEvents()
        {
            Aggregate.ApplyEvents();
            //try
            //{
            foreach (var e in GivenEvents())
                    ((IAggregate) Aggregate).ApplyEvent(e);
            //}
            //catch (Exception ex)
            //{
            //    if (ex.GetType() == ExpectedException || ex.InnerException?.GetType() == ExpectedException)
            //        Assert.Pass("Встречена ожидаемое исключение " + ex.GetType());
            //    throw;
            //}
            //if (ExpectedException != null)
            //    Assert.Fail($"Ожидаемое исключение {ExpectedException} не было получено");
        }

        protected TAggregate Aggregate;

        protected virtual IEnumerable<DomainEvent> GivenEvents()
        {
            return Enumerable.Empty<DomainEvent>();
        }
        protected static Fixture Data { get; private set; } = new Fixture();
        protected virtual Type ExpectedException { get; }
        private readonly AggregateFactory aggregateFactory = new AggregateFactory();
    }
}