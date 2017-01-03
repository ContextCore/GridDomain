using System;
using GridDomain.Common;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    [TestFixture]
    class SkuStock_reserve_renew_tests
    {
        private Scenario<SkuStock, SkuStockCommandsHandler> _scenario;
        private DateTime _expirationDate;

        [SetUp]
        public void Given_sku_stock_with_amount_and_reserve_When_renew_reserve()
        {
            Guid aggregateId = Guid.NewGuid();
            var reservationId = Guid.NewGuid();
            var reservationStartTime = BusinessDateTime.Now;
            var reserveTime = TimeSpan.FromMilliseconds(100);
            _expirationDate = reservationStartTime + reserveTime;
          
            _scenario = Scenario.New<SkuStock, SkuStockCommandsHandler>()
                                .Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), 50, reserveTime),
                                       new StockAdded(aggregateId, 10, "test batch 2"),
                                       new StockReserved(aggregateId, reservationId, _expirationDate, 5),
                                       new FutureEventScheduledEvent(reservationId,
                                                                     aggregateId,
                                                                     _expirationDate,
                                                                     new ReserveExpired(aggregateId,reservationId)))
                                .When(new ReserveStockCommand(aggregateId,
                                                              Guid.NewGuid(),
                                                              10,
                                                              Guid.NewGuid(),
                                                              reservationStartTime)
                                      );

            _scenario.Run();
        }

       [Test]
       public void Then_stock_renewed_event_should_be_raised()
       {
           var reserveStockCommand = _scenario.GivenCommand<ReserveStockCommand>();
           var oldReserveId = _scenario.GivenEvent<StockReserved>().ReservationId;
           var newReserveId = reserveStockCommand.ReservationId;
           var sourceId = _scenario.Aggregate.Id;
                  
           _scenario.Then(new ReserveRenewed(sourceId,
                                             oldReserveId,
                                             newReserveId),

                          new FutureEventScheduledEvent(newReserveId,
                                                        sourceId,
                                                        _expirationDate,
                                                        new ReserveExpired(sourceId,newReserveId)));
                                            
           _scenario.Check();
       }
    }
}