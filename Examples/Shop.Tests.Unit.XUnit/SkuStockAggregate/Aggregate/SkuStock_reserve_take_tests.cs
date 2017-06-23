using System;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.Aggregate
{
   
    internal class SkuStock_reserve_take_tests
    {
        public SkuStock_reserve_take_tests()// Given_sku_stock_with_amount_When_reserve_first_time()
        {
            var aggregateId = Guid.NewGuid();

            _scenario =
                Scenario.New<SkuStock, SkuStockCommandsHandler>()
                        .Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), 50, TimeSpan.FromMilliseconds(100)),
                               new StockAdded(aggregateId, 10, "test batch 2"))
                        .When(_takeCommand = new TakeFromStockCommand(aggregateId, 5));
            _initialStock = _scenario.Aggregate.Quantity;
            _scenario.Run();
        }

        private TakeFromStockCommand _takeCommand;
        private int _initialStock;
        private AggregateScenario<SkuStock, SkuStockCommandsHandler> _scenario;

       [Fact]
        public void Then_Aggregate_Quantity_should_be_reduced_by_take_amount()
        {
            Assert.Equal(_initialStock - _takeCommand.Quantity, _scenario.Aggregate.Quantity);
        }

       [Fact]
        public void Then_no_future_events_should_be_made()
        {
             Assert.Empty(_scenario.Aggregate.FutureEvents);
        }

       [Fact]
        public void Then_no_reservations_should_be_made()
        {
             Assert.Empty(_scenario.Aggregate.Reservations);
        }

       [Fact]
        public void Then_stock_taken_event_should_be_raised()
        {
            _scenario.Then(new StockTaken(_takeCommand.StockId, _takeCommand.Quantity));
            _scenario.Check();
        }
    }
}