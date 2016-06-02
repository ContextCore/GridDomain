using GridDomain.Balance.Domain.AccountAggregate.Commands;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Balance.Domain.AccountAggregate
{
    public class AccountAggregateCommandsHandler : AggregateCommandsHandler<Account>
    {
        public AccountAggregateCommandsHandler()
        {
            Map<CreateAccountCommand>(cmd => cmd.BalanceId,
                cmd => new Account(cmd.BalanceId, cmd.BusinessId));

            Map<ReplenishAccountCommand>(cmd => cmd.BalanceId,
                (cmd, agr) => agr.Replenish(cmd.Amount));

            Map<WithdrawalAccountCommand>(cmd => cmd.BalanceId,
                (cmd, agr) => agr.Withdrawal(cmd.Amount));
        }
    }
}