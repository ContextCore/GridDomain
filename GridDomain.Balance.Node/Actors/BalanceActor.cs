using System;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.AkkaMessaging
{
    public class BalanceActor : AggregateActor<MoneyBalance>,
                                IHandler<CreateBalanceCommand>,
                                IHandler<WithdrawalBalanceCommand>,
                                IHandler<ReplenishBalanceCommand>
    {
        public BalanceActor(Guid id, AggregateFactory factory) : base(id, factory)
        {
            RegisterCommand<CreateBalanceCommand>(cmd => Aggregate = new MoneyBalance(cmd.BalanceId, cmd.BusinessId));
            RegisterCommand<ReplenishBalanceCommand>(cmd => Aggregate.Replenish(cmd.Amount));
            RegisterCommand<WithdrawalBalanceCommand>(cmd => Aggregate.Withdrawal(cmd.Amount));
        }

        public void Handle(CreateBalanceCommand e)
        {
        }

        public void Handle(WithdrawalBalanceCommand e)
        {
        }

        public void Handle(ReplenishBalanceCommand e)
        {
        }
    }
}