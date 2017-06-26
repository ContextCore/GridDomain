using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    public class SkuStock_successfull_creation_tests
    {
        [Fact]
        public async Task When_creating_stock_Then_stock_created_event_is_raised()
        {
            var id = Guid.NewGuid();
            CreateSkuStockCommand cmd;
            var scenario = await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                            .When(cmd = new CreateSkuStockCommand(id, Guid.NewGuid(), 10, "test batch", TimeSpan.FromMinutes(1)))
                                            .Then(new SkuStockCreated(id, cmd.SkuId, cmd.Quantity, cmd.ReserveTime))
                                            .Run()
                                            .Check();

            //Quantity_is_passed_to_aggregate()
            Assert.Equal(cmd.Quantity, scenario.Aggregate.Quantity);
            //SkuId_is_passed_to_aggregate()
            Assert.Equal(cmd.SkuId, scenario.Aggregate.SkuId);
        }
    }
}