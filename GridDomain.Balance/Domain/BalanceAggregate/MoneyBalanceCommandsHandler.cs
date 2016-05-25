using System;
using System.Runtime.InteropServices;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    public class MoneyBalanceCommandsHandler : AggregateCommandsHandler<MoneyBalance>
    {
        public MoneyBalanceCommandsHandler()
        {
            Map<CreateBalanceCommand>(cmd => cmd.BalanceId,
                                      cmd => new MoneyBalance(cmd.BalanceId, cmd.BusinessId));

            Map<ReplenishBalanceCommand>(cmd => cmd.BalanceId,
                                        (cmd, agr) => agr.Replenish(cmd.Amount));

            Map<WithdrawalBalanceCommand>(cmd => cmd.BalanceId,
                                         (cmd, agr) => agr.Withdrawal(cmd.Amount));
        }
    }
}