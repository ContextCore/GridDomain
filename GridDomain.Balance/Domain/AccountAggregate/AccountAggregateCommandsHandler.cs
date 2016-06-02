using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Balance.Domain.BalanceAggregate
{
    public class MoneyBalanceCommandsHandler : AggregateCommandsHandler<Account>
    {
        public MoneyBalanceCommandsHandler()
        {
            Map<CreateBalanceCommand>(cmd => cmd.BalanceId,
                cmd => new Account(cmd.BalanceId, cmd.BusinessId));

            Map<ReplenishBalanceCommand>(cmd => cmd.BalanceId,
                (cmd, agr) => agr.Replenish(cmd.Amount));

            Map<WithdrawalBalanceCommand>(cmd => cmd.BalanceId,
                (cmd, agr) => agr.Withdrawal(cmd.Amount));
        }
    }
}