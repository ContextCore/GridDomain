using System;
using System.Collections.Generic;
using CommonDomain;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Tests
{
    [TestFixture]
    public abstract class HydrationSpecification<TAggregate> where TAggregate : IAggregate
    {
        [SetUp]
        public void When()
        {
            Aggregate = (TAggregate) aggregateFactory.Build(typeof(TAggregate), Guid.NewGuid(), null);
            try
            {
                foreach (var e in GivenEvents())
                    ((IAggregate) Aggregate).ApplyEvent(e);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == ExpectedException || ex.InnerException?.GetType() == ExpectedException)
                    Assert.Pass("Встречена ожидаемое исключение " + ex.GetType());
                throw;
            }
            if (ExpectedException != null)
                Assert.Fail($"Ожидаемое исключение {ExpectedException} не было получено");
        }

        protected TAggregate Aggregate;

        protected abstract IEnumerable<DomainEvent> GivenEvents();

        protected virtual Type ExpectedException { get; }
        private readonly AggregateFactory aggregateFactory = new AggregateFactory();
    }
}