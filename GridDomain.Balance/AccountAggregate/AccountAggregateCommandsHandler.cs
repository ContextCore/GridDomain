using BusinessNews.Domain.AccountAggregate.Commands;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace BusinessNews.Domain.AccountAggregate
{
    public class AccountAggregateCommandsHandler : AggregateCommandsHandler<Account>
    {
        public AccountAggregateCommandsHandler() : base(null)
        {
            Map<CreateAccountCommand>(cmd => cmd.AccountId,
                   cmd => new Account(cmd.AccountId, cmd.BusinessId));

            Map<ReplenishAccountByCardCommand>(cmd => cmd.AccountId,
                (cmd, agr) => agr.Replenish(cmd.Amount));

            Map<PayForBillCommand>(cmd => cmd.AccountId,
                (cmd, agr) => agr.PayBill(cmd.Amount, cmd.BillId));
        }
    }
}