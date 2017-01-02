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
    class SkuStock_replenishment_tests : AggregateCommandsTest<SkuStock, SkuStockCommandsHandler>
    {
        private AddToStockCommand _command;
        private readonly Guid _skuId = Guid.NewGuid();
        private int _initialStock;

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SkuStockCreated(Aggregate.Id,  _skuId,50,TimeSpan.FromMilliseconds(100));
            yield return new StockAdded(Aggregate.Id, 10,"test batch 2");
        }

        [OneTimeSetUp]
        public void When_adding_stock()
        {
            Init();
            _initialStock = Aggregate.Quantity;
            _command = new AddToStockCommand(Aggregate.Id, _skuId, 10, "test batch");
            Execute(_command);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new StockAdded(Aggregate.Id, _command.Quantity, _command.BatchArticle);
        }

        [Test]
        public void Aggregate_quantity_should_be_increased_by_command_amount()
        {
            Assert.AreEqual(_command.Quantity + _initialStock, Aggregate.Quantity);
        }

        [Test]
        public void When_add_negative_amount_should_throw_exception()
        {
            var command = new AddToStockCommand(Aggregate.Id, _skuId, -10, "test batch");

            Assert.Throws<ArgumentException>(() => Execute(command));
        }
        [Test]
        public void When_add_sku_not_belonging_to_stock_should_throw_exception()
        {
            var command = new AddToStockCommand(Aggregate.Id, Guid.NewGuid(), 10, "test batch");

            Assert.Throws<InvalidSkuAddException>(() => Execute(command));
        }
    }
}