using System;
using GridDomain.Common;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    [TestFixture]
    class SkuStock_reserve_take_tests
    {
        private ReserveStockCommand _reserveStockCommand;
        private int _initialStock;
        private readonly TimeSpan _reserveTime = TimeSpan.FromMilliseconds(100);
        private Scenario<SkuStock, SkuStockCommandsHandler> _scenario;
        private DateTime _expirationDate;
        private Reservation _aggregateReserve;

        [SetUp]
        public void Given_sku_stock_with_amount_When_reserve_first_time()
        {
            Guid aggregateId = Guid.NewGuid();

            var reservationStartTime = BusinessDateTime.Now;

            _reserveStockCommand = new ReserveStockCommand(aggregateId,
                Guid.NewGuid(),
                10,
                Guid.NewGuid(),
                reservationStartTime);

            _expirationDate = reservationStartTime + _reserveTime;

            _scenario = Scenario.New<SkuStock, SkuStockCommandsHandler>()
                .Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), 50, _reserveTime),
                    new StockAdded(aggregateId, 10, "test batch 2"))
                .When(_reserveStockCommand);

            _initialStock = _scenario.Aggregate.Quantity;
            _scenario.Run();
            _aggregateReserve = _scenario.Aggregate.Reservations[_reserveStockCommand.ReservationId];
        }

        [Test]
        public void Then_stock_reserved_event_should_be_raised()
        {
            _scenario.Then(new StockReserved(_reserveStockCommand.StockId,
                _reserveStockCommand.ReservationId,
                _expirationDate,
                _reserveStockCommand.Quantity));
            _scenario.Check();
        }

        [Test]
        public void Then_reservation_should_be_added_in_aggregate()
        {
            CollectionAssert.IsNotEmpty(_scenario.Aggregate.Reservations);
        }

        [Test]
        public void Then_aggregate_reservation_for_stock_should_have_correct_expiration_date()
        {
            Assert.AreEqual(_expirationDate, _aggregateReserve.ExpirationDate);
        }

        [Test]
        public void Then_aggregate_reservation_for_stock_should_have_correct_id_date()
        {
            Assert.AreEqual(_reserveStockCommand.ReservationId, _aggregateReserve.Id);
        }

        [Test]
        public void Then_aggregate_reservation_for_stock_should_have_correct_quanity()
        {
            Assert.AreEqual(_reserveStockCommand.Quantity, _aggregateReserve.Quantity);
        }

        [Test]
        public void Aggregate_quantity_should_be_decreased_by_command_amount()
        {
            Assert.AreEqual(_initialStock - _reserveStockCommand.Quantity, _scenario.Aggregate.Quantity);
        }
    }
}