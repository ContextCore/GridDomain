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
        }
    }
}