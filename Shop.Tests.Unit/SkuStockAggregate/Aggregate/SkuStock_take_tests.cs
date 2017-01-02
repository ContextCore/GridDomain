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
    class SkuStock_take_tests : AggregateCommandsTest<SkuStock, SkuStockCommandsHandler>
    {
        private TakeFromStockCommand _command;
        private readonly Guid _skuId = Guid.NewGuid();
        private int _initialStock;

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new SkuStockCreated(Aggregate.Id, _skuId, 50, TimeSpan.FromMilliseconds(100));
            yield return new StockAdded(Aggregate.Id, 10, "test batch 2");
        }

        [OneTimeSetUp]
        public void When_adding_stock()
        {
            Init();
            _initialStock = Aggregate.Quantity;
            _command = new TakeFromStockCommand(Aggregate.Id, 10);
            Execute(_command);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new StockTaken(Aggregate.Id, _command.Quantity);
        }

        [Test]
        public void Aggregate_quantity_should_be_decreased_by_command_amount()
        {
            Assert.AreEqual(_initialStock - _command.Quantity, Aggregate.Quantity);
        }

        [Test]
        public void When_take_too_many_should_throw_exception()
        {
            var command = new TakeFromStockCommand(Aggregate.Id, 100);

            Assert.Throws<OutOfStockException>(() => Execute(command));
        }
    }
}