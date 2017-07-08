using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Scheduling.FutureEvents;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    public class SkuStock_reserve_renew_tests
    {
        [Fact]
        public async Task Given_sku_stock_with_amount_and_reserve_When_renew_reserve()
        {
            var aggregateId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var reservationStartTime = BusinessDateTime.Now;
            var reserveTime = TimeSpan.FromMilliseconds(100);
            var expirationDate = reservationStartTime + reserveTime;
            ReserveStockCommand reserveStockCommand;
            StockReserved stockReservedEvent;

            var scenario = AggregateScenario.New<SkuStock, SkuStockCommandsHandler>();

            await scenario.Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), 50, reserveTime),
                                 new StockAdded(aggregateId, 10, "test batch 2"),
                                 stockReservedEvent = new StockReserved(aggregateId, customerId, expirationDate, 5),
                                 new FutureEventScheduledEvent(Guid.NewGuid(),
                                                               aggregateId,
                                                               expirationDate,
                                                               new ReserveExpired(aggregateId, customerId)))
                          .When(reserveStockCommand = new ReserveStockCommand(aggregateId, customerId, 10, reservationStartTime))
                          .Then(new ReserveRenewed(scenario.Aggregate.Id, customerId),
                                new FutureEventCanceledEvent(Any.GUID, scenario.Aggregate.Id, nameof(SkuStock)),
                                new StockReserved(scenario.Aggregate.Id,
                                                  customerId,
                                                  expirationDate,
                                                  reserveStockCommand.Quantity + stockReservedEvent.Quantity),
                                new FutureEventScheduledEvent(Any.GUID, scenario.Aggregate.Id, expirationDate, new ReserveExpired(scenario.Aggregate.Id, customerId)))
                          .Run()
                          .Check();

            var reservation = scenario.Aggregate.Reservations[customerId];

            //Then_aggregate_reservation_quantity_is_sum_of_initial_and_new_reservations()
            Assert.Equal(reserveStockCommand.Quantity + stockReservedEvent.Quantity, reservation.Quantity);
            //Then_Aggregate_Reservation_should_have_new_expiration_time()
            Assert.Equal(expirationDate, reservation.ExpirationDate);
            //Then_Aggregate_Reservation_should_remain_for_same_customer()
            Assert.NotNull(reservation);
            //Then_correct_events_should_be_raised()
        }
    }
}