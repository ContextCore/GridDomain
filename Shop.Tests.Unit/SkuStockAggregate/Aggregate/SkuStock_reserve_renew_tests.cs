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
        private AggregateScenario<SkuStock, SkuStockCommandsHandler> _scenario;
        private DateTime _expirationDate;
        private ReserveStockCommand _reserveStockCommand;
        private StockReserved _stockReservedEvent;

        [SetUp]
        public void Given_sku_stock_with_amount_and_reserve_When_renew_reserve()
        {
            Guid aggregateId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var reservationStartTime = BusinessDateTime.Now;
            var reserveTime = TimeSpan.FromMilliseconds(100);
            _expirationDate = reservationStartTime + reserveTime;
          
            _scenario = Scenario.New<SkuStock, SkuStockCommandsHandler>()
                                .Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), 50, reserveTime),
                                       new StockAdded(aggregateId, 10, "test batch 2"),
                                       _stockReservedEvent = 
                                       new StockReserved(aggregateId, customerId, _expirationDate, 5),
                                       new FutureEventScheduledEvent(Guid.NewGuid(),
                                                                     aggregateId,
                                                                     _expirationDate,
                                                                     new ReserveExpired(aggregateId, customerId)))

                                .When(_reserveStockCommand = new ReserveStockCommand(aggregateId,
                                                                                     customerId,
                                                                                     10,
                                                                                     reservationStartTime)
                                      );

            _scenario.Run();
        }

       [Test]
       public void Then_correct_events_should_be_raised()
        { 
           var customerId = _stockReservedEvent.ReserveId;
      
           var sourceId = _scenario.Aggregate.Id;

           _scenario.Then(new FutureEventCanceledEvent(Any.GUID, sourceId),
                          new ReserveRenewed(sourceId,
                                             customerId),

                          new StockReserved(sourceId,
                                            customerId,
                                            _expirationDate,
                                            _reserveStockCommand.Quantity + _stockReservedEvent.Quantity),

                          new FutureEventScheduledEvent(Any.GUID,
                                                        sourceId,
                                                        _expirationDate,
                                                        new ReserveExpired(sourceId,customerId)));
                                            
           _scenario.Check();
       }

        private Reservation NewReservation
        {
            get
            {
                var reserveStockCommand = _scenario.GivenCommand<ReserveStockCommand>();
                Reservation reservation;
                _scenario.Aggregate.Reservations.TryGetValue(reserveStockCommand.CustomerId, out reservation);
                return reservation;
            }
        }

        [Test]
        public void Then_Aggregate_Reservation_should_remain_for_same_customer()
        {
             Assert.NotNull(NewReservation);
        }

        [Test]
        public void Then_Aggregate_Reservation_should_have_new_expiration_time()
        {
            Assert.AreEqual(_expirationDate, NewReservation.ExpirationDate);
        }

        [Test]
        public void Then_aggregate_reservation_quantity_is_sum_of_initial_and_new_reservations()
        {
            Assert.AreEqual(_reserveStockCommand.Quantity + _stockReservedEvent.Quantity, NewReservation.Quantity);
        }
    }
}