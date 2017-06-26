using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using NMoneys;
using Shop.Domain.Aggregates.SkuAggregate;
using Shop.Domain.Aggregates.SkuAggregate.Commands;
using Shop.Domain.Aggregates.SkuAggregate.Events;
using Shop.Infrastructure;
using Xunit;

namespace Shop.Tests.Unit.SkuAggregate.Aggregate
{
    public class Sku_creation_tests
    {
        [Fact]
        public async Task When_creating_new_sku()
        {
            var id = Guid.NewGuid();
            CreateNewSkuCommand cmd;

            await AggregateScenario.New(new SkuCommandsHandler(new InMemorySequenceProvider()))
                                   .When(cmd = new CreateNewSkuCommand("testSku", "tesstArticle", id, new Money(100)))
                                   .Then(new SkuCreated(id, cmd.Name, cmd.Article, 1, new Money(100)))
                                   .Run()
                                   .Check();
        }
    }
}