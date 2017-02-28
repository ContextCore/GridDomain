using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.Aggregates.SkuAggregate.Commands;
using Shop.Infrastructure;

namespace Shop.Domain.Aggregates.SkuAggregate
{
    public class SkuCommandsHandler : AggregateCommandsHandler<Sku>
    {
        public SkuCommandsHandler(ISequenceProvider provider)
        {
            Map<CreateNewSkuCommand>(c => new Sku(c.SkuId, c.Name, c.Article, (int) provider.GetNext("Sku"), c.Price));
        }
    }
}