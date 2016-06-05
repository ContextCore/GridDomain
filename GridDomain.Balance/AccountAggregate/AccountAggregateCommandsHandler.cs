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

            Map<ReplenishAccountByCardCommand>(cmd => cmd.BalanceId,
                (cmd, agr) => agr.Replenish(cmd.Amount));

            Map<PayForBillCommand>(cmd => cmd.BalanceId,
                (cmd, agr) => agr.PayBill(cmd.Amount, cmd.BillId));
        }
    }
}