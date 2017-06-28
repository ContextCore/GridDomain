using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using Shop.Domain.Aggregates.AccountAggregate.Commands;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    public class AccountCommandsHandler : AggregateCommandsHandler<Account>
    {

        public AccountCommandsHandler()
        {
            Map<CreateAccountCommand>(cmd => new Account(cmd.AccountId, cmd.UserId, cmd.Number));

            Map<ReplenishAccountByCardCommand>((cmd, agr) => agr.Replenish(cmd.Amount, cmd.Id));

            Map<PayForOrderCommand>((cmd, agr) => agr.Withdraw(cmd.Amount, cmd.Id));
        }
    }
}