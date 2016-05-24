using System;
using System.Collections.Generic;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.AkkaMessaging
{
    public class BalanceActor : AggregateActor<MoneyBalance>
    {
        public BalanceActor(AggregateFactory factory) : base(factory)
        {
            RegisterCommand<CreateBalanceCommand>(cmd => Aggregate = new MoneyBalance(cmd.BalanceId, cmd.BusinessId));
            RegisterCommand<ReplenishBalanceCommand>(cmd => Aggregate.Replenish(cmd.Amount));
            RegisterCommand<WithdrawalBalanceCommand>(cmd => Aggregate.Withdrawal(cmd.Amount));
        }
    }
}