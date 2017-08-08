using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.UserAggregate.Commands
{
    public class BuySkuNowCommand : Command
    {
        public BuySkuNowCommand(Guid userId, Guid skuId, int quantity) : base(userId)
        {
            SkuId = skuId;
            Quantity = quantity;
        }

        public Guid SkuId { get; }
        public Guid UserId => AggregateId;
        public int Quantity { get; }
    }
}