using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.Aggregate
{
    public class SkuStock_replenishment_tests
    {
        private readonly Guid _skuId = Guid.NewGuid();

        [Fact]
        public async Task When_adding_stock_Aggregate_quantity_should_be_increased_by_command_amount()
        {
            AddToStockCommand command;
            SkuStockCreated created;
            StockAdded added;
            var id = Guid.NewGuid();
            var scenario = await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                            .Given(created = new SkuStockCreated(id, _skuId, 50, TimeSpan.FromMilliseconds(100)),
                                                   added = new StockAdded(id, 10, "test batch 2"))
                                            .When(command = new AddToStockCommand(id, _skuId, 10, "test batch"))
                                            .Then(new StockAdded(id, command.Quantity, command.BatchArticle))
                                            .Run();

            Assert.Equal(command.Quantity + created.Quantity + added.Quantity, scenario.Aggregate.Quantity);
        }

        [Fact]
        public async Task When_add_negative_amount_should_throw_exception()
        {
            var id = Guid.NewGuid();
            await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                   .Given(new SkuStockCreated(id, _skuId, 50, TimeSpan.FromMilliseconds(100)))
                                   .When(new AddToStockCommand(id, _skuId, -10, "test batch"))
                                   .Run()
                                   .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public async Task When_add_sku_not_belonging_to_stock_should_throw_exception()
        {
            var id = Guid.NewGuid();
            await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                   .Given(new SkuStockCreated(id, _skuId, 50, TimeSpan.FromMilliseconds(100)))
                                   .When(new AddToStockCommand(id, Guid.NewGuid(), 10, "test batch"))
                                   .Run()
                                   .ShouldThrow<InvalidSkuAddException>();
        }
    }
}