using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.Aggregates.AccountAggregate.Commands;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    public class AccountCommandsHandler : AggregateCommandsHandler<Account>
    {
        public AccountCommandsHandler() : base(null)
        {
            Map<CreateAccountCommand>(cmd => cmd.AccountId,
                   cmd => new Account(cmd.AccountId, cmd.BusinessId));

            Map<ReplenishAccountByCardCommand>(cmd => cmd.AccountId,
                (cmd, agr) => agr.Replenish(cmd.Amount));

            Map<PayForBillCommand>(cmd => cmd.AccountId,
                (cmd, agr) => agr.Withdraw(cmd.Amount));
        }
    }
}