using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.Aggregate
{
   
    internal class SkuStock_reserve_first_time_tests
    {
        public SkuStock_reserve_first_time_tests()// Given_sku_stock_with_amount_When_reserve_first_time()
        {
            var aggregateId = Guid.NewGuid();

            var reservationStartTime = BusinessDateTime.Now;

            _reserveStockCommand = new ReserveStockCommand(aggregateId, Guid.NewGuid(), 10, reservationStartTime);

            _expirationDate = reservationStartTime + _reserveTime;

            _scenario =
                Scenario.New<SkuStock, SkuStockCommandsHandler>()
                        .Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), 50, _reserveTime),
                               new StockAdded(aggregateId, 10, "test batch 2"))
                        .When(_reserveStockCommand);

            _initialStock = _scenario.Aggregate.Quantity;
            _scenario.Run();
            _scenario.Aggregate.Reservations.TryGetValue(_reserveStockCommand.CustomerId, out _aggregateReserve);

            if (_aggregateReserve != null)
                _reserveExpirationFutureEvent = _scenario.Aggregate.FutureEvents.FirstOrDefault();
            _reserveExpiredEvent = _reserveExpirationFutureEvent?.Event as ReserveExpired;
        }

        private ReserveStockCommand _reserveStockCommand;
        private int _initialStock;
        private readonly TimeSpan _reserveTime = TimeSpan.FromMilliseconds(100);
        private AggregateScenario<SkuStock, SkuStockCommandsHandler> _scenario;
        private DateTime _expirationDate;
        private Reservation _aggregateReserve;
        private FutureEventScheduledEvent _reserveExpirationFutureEvent;
        private ReserveExpired _reserveExpiredEvent;

       [Fact]
        public void Aggregate_quantity_should_be_decreased_by_command_amount()
        {
            Assert.Equal(_initialStock - _reserveStockCommand.Quantity, _scenario.Aggregate.Quantity);
        }

       [Fact]
        public void Reserve_expiration_event_should_exist()
        {
            Assert.NotNull(_reserveExpiredEvent);
        }

       [Fact]
        public void Reserve_expiration_event_should_have_reserve_id()
        {
            Assert.Equal(_reserveStockCommand.CustomerId, _reserveExpiredEvent?.ReserveId);
        }

       [Fact]
        public void Reserve_expiration_future_event_should_exist()
        {
            Assert.NotNull(_reserveExpirationFutureEvent);
        }

       [Fact]
        public void Reserve_expiration_future_event_should_have_reservation_expiration_date()
        {
            Assert.Equal(_aggregateReserve.ExpirationDate, _reserveExpirationFutureEvent.RaiseTime);
        }

       [Fact]
        public void Then_aggregate_reservation_for_stock_should_have_correct_expiration_date()
        {
            Assert.Equal(_expirationDate, _aggregateReserve.ExpirationDate);
        }

       [Fact]
        public void Then_aggregate_reservation_for_stock_should_have_correct_quanity()
        {
            Assert.Equal(_reserveStockCommand.Quantity, _aggregateReserve.Quantity);
        }

       [Fact]
        public void Then_aggregate_reservation_should_be_added()
        {
            Assert.NotNull(_aggregateReserve);
        }

       [Fact]
        public void Then_reservation_should_be_added_in_aggregate()
        {
            Assert.NotEmpty(_scenario.Aggregate.Reservations);
        }

       [Fact]
        public void Then_stock_reserved_event_should_be_raised()
        {
            _scenario.Then(
                           new StockReserved(_reserveStockCommand.StockId,
                                             _reserveStockCommand.CustomerId,
                                             _expirationDate,
                                             _reserveStockCommand.Quantity),
                           new FutureEventScheduledEvent(Any.GUID,
                                                         _scenario.Aggregate.Id,
                                                         _expirationDate,
                                                         new ReserveExpired(_scenario.Aggregate.Id, _reserveStockCommand.CustomerId)));
            _scenario.Check();
        }
    }
}