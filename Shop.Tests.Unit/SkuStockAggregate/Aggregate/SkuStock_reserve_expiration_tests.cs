using System;
using System.Threading;
using GridDomain.Common;
using GridDomain.EventSourcing.FutureEvents;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    [TestFixture]
    class SkuStock_reserve_expiration_tests
    {
        private int _initialStock;
        private Scenario<SkuStock, SkuStockCommandsHandler> _scenario;
        private DateTime _expirationDate;
        private Guid _aggregateId;
        private RaiseScheduledDomainEventCommand _raiseEventCommand;
        private StockReserved _stockReserved;
        private StockAdded _stockAdded;
        private SkuStockCreated _stockCreated;
        private Guid _customerId;

        [SetUp]
        public void Given_sku_stock_with_amount_and_reserve_When_reserve_expires()
        {
             _aggregateId = Guid.NewGuid();
             _customerId = Guid.NewGuid();
             var reservationStartTime = BusinessDateTime.Now;
             var reserveTime = TimeSpan.FromMilliseconds(100);

             _expirationDate = reservationStartTime + reserveTime;

            var futureEventId = Guid.NewGuid();
            _scenario = Scenario.New<SkuStock, SkuStockCommandsHandler>()
                                .Given(_stockCreated = new SkuStockCreated(_aggregateId, Guid.NewGuid(), 50, reserveTime),
                                       _stockAdded = new StockAdded(_aggregateId, 10, "test batch 2"),
                                       _stockReserved = 
                                       new StockReserved(_aggregateId, _customerId, _expirationDate, 5),
                                       new FutureEventScheduledEvent(futureEventId, 
                                                                     _aggregateId,
                                                                     _expirationDate,
                                                                     new ReserveExpired(_aggregateId, _customerId)))
                                .When(_raiseEventCommand = new RaiseScheduledDomainEventCommand(futureEventId, _aggregateId, Guid.NewGuid()));


            _initialStock = _scenario.Aggregate.Quantity;

            _scenario.Run();

        }

        [Test]
        public void Then_stock_reserve_expired_event_should_be_raised()
        {
            _scenario.Then(new ReserveExpired(_aggregateId,
                                              _customerId),
                           new FutureEventOccuredEvent(Any.GUID, 
                                                       _raiseEventCommand.FutureEventId,
                                                       _aggregateId)
                           );
            _scenario.Check();
        }

        [Test]
        public void Then_reservation_should_be_removed_from_aggregate()
        {
            CollectionAssert.IsEmpty(_scenario.Aggregate.Reservations);
        }

        [Test]
        public void Then_aggregate_stock_quanity_should_be_increased_by_reservation_quantity()
        {
            Assert.AreEqual(_initialStock + _stockReserved.Quantity, _scenario.Aggregate.Quantity);
        }

        [Test]
        public void Aggregate_quantity_should_be_the_same_as_when_created()
        {
            Assert.AreEqual(_stockAdded.Quantity + _stockCreated.Quantity, _scenario.Aggregate.Quantity);
        }
    }
}