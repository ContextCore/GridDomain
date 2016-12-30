using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuAggregate;
using Shop.Domain.Aggregates.SkuAggregate.Commands;
using Shop.Domain.Aggregates.SkuAggregate.Events;
using Shop.Infrastructure;

namespace Shop.Tests.Unit.SkuAggregate.Aggregate
{
    [TestFixture]
    class Sku_creation_tests : AggregateCommandsTest<Sku,SkuCommandHandler>
    {
        private static readonly InMemorySequenceProvider SequenceProvider = new InMemorySequenceProvider();
        private CreateNewSkuCommand _cmd;

        protected override SkuCommandHandler CreateCommandsHandler()
        {
            return new SkuCommandHandler(SequenceProvider);
        }
        
        [Test]
        public void When_creating_new_sku()
        {
            Init();
            _cmd = new CreateNewSkuCommand("testSku", "tesstArticle", Aggregate.Id);
            Execute(_cmd);
        }

        protected override IEnumerable<DomainEvent> Expected()
        {
            yield return new SkuCreated(Aggregate.Id,_cmd.Name,_cmd.Article,1);
        }
    }
}
