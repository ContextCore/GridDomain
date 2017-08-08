using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    public class SkuStock_reserve_take_tests
    {
        [Fact]
        public async Task Given_sku_stock_with_amount_When_reserve_first_time()
        {
            var aggregateId = Guid.NewGuid();
            TakeFromStockCommand takeCommand;
            var scenario = await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                                   .Given(new SkuStockCreated(aggregateId, Guid.NewGuid(), 50, TimeSpan.FromMilliseconds(100)),
                                                          new StockAdded(aggregateId, 10, "test batch 2"))
                                                   .When(takeCommand = new TakeFromStockCommand(aggregateId, 5))
                                                   .Then(new StockTaken(takeCommand.StockId, takeCommand.Quantity))
                                                   .Run()
                                                   .Check();

            //Then_Aggregate_Quantity_should_be_reduced_by_take_amount()
            Assert.Equal(60 - takeCommand.Quantity, scenario.Aggregate.Quantity);
            //Then_no_future_events_should_be_made()
            Assert.Empty(scenario.Aggregate.FutureEvents);
            //Then_no_reservations_should_be_made()
            Assert.Empty(scenario.Aggregate.Reservations);
        }
    }
}