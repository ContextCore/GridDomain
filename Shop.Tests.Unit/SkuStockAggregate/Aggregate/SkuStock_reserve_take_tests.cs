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
        private TakeFromStockCommand _takeCommand;
        private int _initialStock;
        private Scenario<SkuStock, SkuStockCommandsHandler> _scenario;

        [SetUp]
        public void Given_sku_stock_with_amount_When_reserve_first_time()
        {
            Guid aggregateId = Guid.NewGuid();

            _scenario = Scenario.New<SkuStock, SkuStockCommandsHandler>()
                                .Given(new SkuStockCreated(aggregateId, 
                                                           Guid.NewGuid(),
                                                           50, 
                                                           TimeSpan.FromMilliseconds(100)),
                                       new StockAdded(aggregateId, 
                                                      10, 
                                                      "test batch 2"))
                                .When(_takeCommand = new TakeFromStockCommand(aggregateId,5));
            _initialStock = _scenario.Aggregate.Quantity;
            _scenario.Run();
        }

        [Test]
        public void Then_stock_taken_event_should_be_raised()
        {
            _scenario.Then(new StockTaken(_takeCommand.StockId,
                                          _takeCommand.Quantity));
            _scenario.Check();
        }

        [Test]
        public void Then_Aggregate_Quantity_should_be_reduced_by_take_amount()
        {
            Assert.AreEqual(_initialStock - _takeCommand.Quantity, _scenario.Aggregate.Quantity);
        }

        [Test]
        public void Then_no_reservations_should_be_made()
        {
            CollectionAssert.IsEmpty(_scenario.Aggregate.Reservations);
        }
        [Test]
        public void Then_no_future_events_should_be_made()
        {
            CollectionAssert.IsEmpty(_scenario.Aggregate.FutureEvents);
        }

    }
}