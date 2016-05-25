using System;
using System.Runtime.InteropServices;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.AkkaMessaging
{
    //public class BalanceActor : AggregateActor<MoneyBalance>
    //{
    //    public BalanceActor(AggregateFactory factory, IPublisher publisher) : base(factory,publisher)
    //    {
    //        RegisterCommand<CreateBalanceCommand>(cmd => Aggregate = new MoneyBalance(cmd.BalanceId, cmd.BusinessId));
    //        RegisterCommand<ReplenishBalanceCommand>(cmd => Aggregate.Replenish(cmd.Amount));
    //        RegisterCommand<WithdrawalBalanceCommand>(cmd => Aggregate.Withdrawal(cmd.Amount));
    //    }
    //}


    class MoneyBalanceCommandsHandler : AggregateCommandsExecutor<MoneyBalance>
    {
        public MoneyBalanceCommandsHandler(Func<MoneyBalance> aggregateSelector) : base(aggregateSelector)
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