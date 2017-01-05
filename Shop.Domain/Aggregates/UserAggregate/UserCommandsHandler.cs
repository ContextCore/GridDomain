using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.Aggregates.UserAggregate.Commands;

namespace Shop.Domain.Aggregates.UserAggregate
{
    public class UserCommandsHandler : AggregateCommandsHandler<User>
    {
        public UserCommandsHandler()
        {
            Map<CreateUserCommand>(c => c.UserId,
                                   c => new User(c.UserId,c.Login,c.AccountId));

            Map<CompletePendingOrderCommand>(c => c.UserId,
                                            (c,a) => a.CompleteOrder(c.OrderId) );

            Map<BuySkuNowCommand>(c => c.UserId,
                                 (c,a) => a.BuyNow(c.SkuId,c.Quantity));

            Map<CancelPendingOrderCommand>(c => c.UserId,   
                                          (c,a) => a.CancelOrder(c.OrderId));
        }
    }
}