using System;
using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    [TestFixture]
    [Ignore("Under development")]
    class SkuStock_reserve_tests : AggregateCommandsTest<SkuStock, SkuStockCommandsHandler>
    {
        private ReserveStockCommand _command;
        private readonly Guid _skuId = Guid.NewGuid();
        private int _initialStock;
        private readonly TimeSpan _reserveTime = TimeSpan.FromMilliseconds(100);

        protected IEnumerable<DomainEvent> Given_stock_with_amount()
        {
            yield return new SkuStockCreated(Aggregate.Id, _skuId, 50, _reserveTime);
            yield return new StockAdded(Aggregate.Id, 10, "test batch 2");
        }

        [Test]
        public void When_reserve_first_time()
        {
            _initialStock = Aggregate.Quantity;
            _command = new ReserveStockCommand(Aggregate.Id, Guid.NewGuid(),10, Guid.NewGuid());
            RunScenario(Given_stock_with_amount(), Expected(),_command);
        }

        protected IEnumerable<DomainEvent> Expected()
        {
            yield return new StockReserved(Aggregate.Id,_command.ReservationId,BusinessDateTime.UtcNow + _reserveTime, _command.Quantity);
        }

        [Test]
        public void Aggregate_quantity_should_be_decreased_by_command_amount()
        {
            Assert.AreEqual(_command.Quantity - _initialStock, Aggregate.Quantity);
        }

        [Test]
        public void When_take_too_many_should_throw_exception()
        {
            var command = new TakeFromStockCommand(Aggregate.Id, 100);

            Assert.Throws<OutOfStockException>(() => Execute(command));
        }
    }
}