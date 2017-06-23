using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit.CommandsExecution;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.Aggregate
{
    public class SkuStock_take_tests
    {
        private readonly Guid _skuId = Guid.NewGuid();

        [Fact]
        public void SkuStock_take_When_adding_stock()
        {
            var id = Guid.NewGuid();
            TakeFromStockCommand cmd;

            var scenario = AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                            .Given(new SkuStockCreated(id, _skuId, 50, TimeSpan.FromMilliseconds(100)),
                                                   new StockAdded(id, 10, "test batch 2"))
                                            .When(cmd = new TakeFromStockCommand(id, 10))
                                            .Then(new StockTaken(id, cmd.Quantity))
                                            .Run()
                                            .Check();

            //Aggregate_quantity_should_be_decreased_by_command_amount()
            Assert.Equal(60 - cmd.Quantity, scenario.Aggregate.Quantity);
        }

        [Fact]
        public async Task When_take_too_many_should_throw_exception()
        {
            var id = Guid.NewGuid();
            await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                   .Given(new SkuStockCreated(id, _skuId, 50, TimeSpan.FromMilliseconds(100)),
                                          new StockAdded(id, 10, "test batch 2"))
                                   .When(new TakeFromStockCommand(id, 100))
                                   .RunAsync()
                                   .ShouldThrow<OutOfStockException>();
        }
    }
}