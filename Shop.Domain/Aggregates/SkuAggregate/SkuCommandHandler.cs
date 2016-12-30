using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using Microsoft.Practices.Unity;
using Shop.Domain.Aggregates.SkuAggregate.Commands;
using Shop.Infrastructure;

namespace Shop.Domain.Aggregates.SkuAggregate
{
    public class SkuCommandHandler : AggregateCommandsHandler<Sku>
    {
        public SkuCommandHandler(ISequenceProvider provider)
        {
            Map<CreateNewSkuCommand>(c => c.SkuId,
                c => new Sku(c.SkuId,c.Name,c.Article, (int)provider.GetNext("Sku")));
        }
    }
}