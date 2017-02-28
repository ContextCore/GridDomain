using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.Aggregates.UserAggregate.Commands;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public class UserCommandsHandler : AggregateCommandsHandler<User>
    {
        public UserCommandsHandler(IDefaultStockProvider stockProvider)
        {
            Map<CreateUserCommand>(c => new User(c.UserId, c.Login, c.AccountId));

            Map<CompletePendingOrderCommand>((c, a) => a.CompleteOrder(c.OrderId));

            Map<BuySkuNowCommand>((c, a) => a.BuyNow(c.SkuId, c.Quantity, stockProvider));

            Map<CancelPendingOrderCommand>((c, a) => a.CancelOrder(c.OrderId));
        }
    }
}