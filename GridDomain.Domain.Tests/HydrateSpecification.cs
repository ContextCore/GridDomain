using System;
using System.Collections.Generic;
using CommonDomain;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Domain.Tests
{
    [TestFixture]
    public abstract class HydrateSpecification<TAggregate> where TAggregate : IAggregate
    {
        protected TAggregate Aggregate;

        protected abstract IEnumerable<DomainEvent> CreateEvent { get; }

        protected virtual Type ExpectedException { get; }
        private AggregateFactory  aggregateFactory = new AggregateFactory();
        [SetUp]
        public void When()
        {
            Aggregate = (TAggregate)aggregateFactory.Build(typeof(TAggregate), Guid.NewGuid(), null);
            try
            {
                foreach(var e in CreateEvent)
                  ((IAggregate)Aggregate).ApplyEvent(e);
            }
            catch (Exception ex)
            {
                if(ex.GetType() == ExpectedException || ex.InnerException?.GetType() == ExpectedException)
                    Assert.Pass("Встречена ожидаемое исключение " + ex.GetType());
                throw;
            }
            if (ExpectedException != null)
                Assert.Fail($"Ожидаемое исключение {ExpectedException} не было получено");
        }
    }
}