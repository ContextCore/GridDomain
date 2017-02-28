using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuAggregate;
using Shop.Domain.Aggregates.SkuAggregate.Commands;
using Shop.Domain.Aggregates.SkuAggregate.Events;
using Shop.Infrastructure;

namespace Shop.Tests.Unit.SkuAggregate.Aggregate
{
    [TestFixture]
    internal class Sku_creation_tests : AggregateCommandsTest<Sku, SkuCommandsHandler>
    {
        private static readonly InMemorySequenceProvider SequenceProvider = new InMemorySequenceProvider();
        private CreateNewSkuCommand _cmd;

        protected override SkuCommandsHandler CreateCommandsHandler()
        {
            return new SkuCommandsHandler(SequenceProvider);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new SkuCreated(Aggregate.Id, _cmd.Name, _cmd.Article, 1, new Money(100));
        }

        [Test]
        public void When_creating_new_sku()
        {
            Init();
            _cmd = new CreateNewSkuCommand("testSku", "tesstArticle", Aggregate.Id, new Money(100));
            Execute(_cmd);
        }
    }
}