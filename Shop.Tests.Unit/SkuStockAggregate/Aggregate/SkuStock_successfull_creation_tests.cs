using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    [TestFixture]
    internal class SkuStock_successfull_creation_tests : AggregateCommandsTest<SkuStock, SkuStockCommandsHandler>
    {
        private CreateSkuStockCommand _command;

        [OneTimeSetUp]
        public void When_creating_stock()
        {
            Init();
            _command = new CreateSkuStockCommand(Aggregate.Id, Guid.NewGuid(), 10, "test batch", TimeSpan.FromMinutes(1));
            Execute(_command);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new SkuStockCreated(Aggregate.Id, _command.SkuId, _command.Quantity, _command.ReserveTime);
        }

        [Test]
        public void Quantity_is_passed_to_aggregate()
        {
            Assert.AreEqual(_command.Quantity, Aggregate.Quantity);
        }

        [Test]
        public void SkuId_is_passed_to_aggregate()
        {
            Assert.AreEqual(_command.SkuId, Aggregate.SkuId);
        }
    }
}