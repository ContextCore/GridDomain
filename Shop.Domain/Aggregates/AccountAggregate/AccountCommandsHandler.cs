using GridDomain.CQRS.Messaging.MessageRouting;
using Shop.Domain.Aggregates.AccountAggregate.Commands;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    public class AccountCommandsHandler : AggregateCommandsHandler<Account>
    {
        public AccountCommandsHandler() : base(null)
        {
            Map<CreateAccountCommand>(cmd => cmd.AccountId,
                   cmd => new Account(cmd.AccountId, cmd.UserId, cmd.Number));

            Map<ReplenishAccountByCardCommand>(cmd => cmd.AccountId,
                (cmd, agr) => agr.Replenish(cmd.Amount, cmd.Id));

            Map<PayForBillCommand>(cmd => cmd.AccountId,
                (cmd, agr) => agr.Withdraw(cmd.Amount, cmd.Id));
        }
    }
}