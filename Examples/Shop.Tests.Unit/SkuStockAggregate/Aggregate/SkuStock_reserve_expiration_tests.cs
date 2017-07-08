using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Scheduling.FutureEvents;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    public class SkuStock_reserve_expiration_tests
    {
        [Fact]
        public async Task Given_sku_stock_with_amount_and_reserve_When_reserve_expires()
        {
            var aggregateId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var reservationStartTime = BusinessDateTime.Now;
            var reserveTime = TimeSpan.FromMilliseconds(100);
            var expirationDate = reservationStartTime + reserveTime;
            var futureEventId = Guid.NewGuid();
            var initialQuantity = 50;
            var addedQuantity = 10;

            var scenario = await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                                  .Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), initialQuantity, reserveTime),
                                                         new StockAdded(aggregateId, addedQuantity, "test batch 2"),
                                                         new StockReserved(aggregateId, customerId, expirationDate, 5),
                                                         new FutureEventScheduledEvent(futureEventId,
                                                                                       aggregateId,
                                                                                       expirationDate,
                                                                                       new ReserveExpired(aggregateId, customerId)))
                                                  .When(new RaiseScheduledDomainEventCommand(futureEventId, aggregateId, Guid.NewGuid()))
                                                  .Then(new ReserveExpired(aggregateId, customerId),
                                                        new FutureEventOccuredEvent(Any.GUID, futureEventId, aggregateId))
                                                  .Run()
                                                  .Check();


            //Then reserved stock quantity should be returned to sku stock
            Assert.Equal(initialQuantity + addedQuantity, scenario.Aggregate.Quantity);
            //Then_reservation should be removed from aggregate
            Assert.Empty(scenario.Aggregate.Reservations);
            //Then expiration events should be raised
        }
    }
}