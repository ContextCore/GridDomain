using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.Aggregate
{
   
    internal class SkuStock_successfull_creation_tests : AggregateCommandsTest<SkuStock, SkuStockCommandsHandler>
    {
        private CreateSkuStockCommand _command;

        public SkuStock_successfull_creation_tests()// When_creating_stock()
        {
            Init();
            _command = new CreateSkuStockCommand(Aggregate.Id, Guid.NewGuid(), 10, "test batch", TimeSpan.FromMinutes(1));
            Execute(_command).Wait();
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new SkuStockCreated(Aggregate.Id, _command.SkuId, _command.Quantity, _command.ReserveTime);
        }

       [Fact]
        public void Quantity_is_passed_to_aggregate()
        {
            Assert.Equal(_command.Quantity, Aggregate.Quantity);
        }

       [Fact]
        public void SkuId_is_passed_to_aggregate()
        {
            Assert.Equal(_command.SkuId, Aggregate.SkuId);
        }
    }
}