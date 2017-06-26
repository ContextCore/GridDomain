using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    public class SkuStock_reserve_first_time_tests
    {
        [Fact]
        public async Task Given_sku_stock_with_amount_When_reserve_first_time()
        {
            var reserveTime = TimeSpan.FromMilliseconds(100);
            Reservation aggregateReserve;
            FutureEventScheduledEvent reserveExpirationFutureEvent = null;
            var aggregateId = Guid.NewGuid();
            var reservationStartTime = BusinessDateTime.Now;

            ReserveStockCommand reserveStockCommand;
            var expirationDate = reservationStartTime + reserveTime;

            var initialQuantity = 50;
            var addedQuantity = 10;

            var scenario = await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                                  .Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), initialQuantity, reserveTime),
                                                         new StockAdded(aggregateId, addedQuantity, "test batch 2"))
                                                  .When(reserveStockCommand = new ReserveStockCommand(aggregateId, Guid.NewGuid(), 10, reservationStartTime))
                                                  .Then(new StockReserved(reserveStockCommand.StockId,
                                                                          reserveStockCommand.CustomerId,
                                                                          expirationDate,
                                                                          reserveStockCommand.Quantity),
                                                        new FutureEventScheduledEvent(Any.GUID,
                                                                                      aggregateId,
                                                                                      expirationDate,
                                                                                      new ReserveExpired(aggregateId, reserveStockCommand.CustomerId)))
                                                  .Run().Check();

            // Then_stock_reserved_event_should_be_raised()

            scenario.Aggregate.Reservations.TryGetValue(reserveStockCommand.CustomerId, out aggregateReserve);
            if (aggregateReserve != null)
                reserveExpirationFutureEvent = scenario.Aggregate.FutureEvents.FirstOrDefault();
            var reserveExpiredEvent = reserveExpirationFutureEvent?.Event as ReserveExpired;
            // Aggregate_quantity_should_be_decreased_by_command_amount()
            Assert.Equal(initialQuantity + addedQuantity - reserveStockCommand.Quantity, scenario.Aggregate.Quantity);
            //Reserve_expiration_event_should_exist()
            Assert.NotNull(reserveExpiredEvent);
            //Reserve_expiration_event_should_have_reserve_id()
            Assert.Equal(reserveStockCommand.CustomerId, reserveExpiredEvent?.ReserveId);
            //Reserve_expiration_future_event_should_exist()
            Assert.NotNull(reserveExpirationFutureEvent);
            // Reserve_expiration_future_event_should_have_reservation_expiration_date()
            Assert.Equal(aggregateReserve.ExpirationDate, reserveExpirationFutureEvent.RaiseTime);
            //Then_aggregate_reservation_for_stock_should_have_correct_expiration_date()
            Assert.Equal(expirationDate, aggregateReserve.ExpirationDate);
            //Then_aggregate_reservation_for_stock_should_have_correct_quanity()
            Assert.Equal(reserveStockCommand.Quantity, aggregateReserve.Quantity);
            //Then_aggregate_reservation_should_be_added()
            Assert.NotNull(aggregateReserve);
            // Then_reservation_should_be_added_in_aggregate()
            Assert.NotEmpty(scenario.Aggregate.Reservations);
        }
    }
}