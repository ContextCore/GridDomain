using System;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using NMoneys;
using Shop.Domain.Aggregates.AccountAggregate.Events;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    public class Account : Aggregate
    {
        public Guid UserId { get; private set; }
        public Money Amount { get; private set; }
        public int Number { get; private set; }

        private Account(Guid id):base(id)
        {
        }

        public Account(Guid id, Guid userId, int number) : this(id)
        {
            RaiseEvent(new AccountCreatedEvent(id, userId, number));
        }

        private void Apply(AccountCreatedEvent e)
        {
            Id = e.SourceId;
            UserId = e.UserId;
            Number = e.AccountNumber;
        }

        private void Apply(AccountReplenish e)
        {
            Amount += e.Amount;
        }

        private void Apply(AccountWithdrawal e)
        {
            Amount -= e.Amount;
        }

        public void Replenish(Money m)
        {
            GuardNegativeMoney(m, "Cant replenish negative amount of money.");
            RaiseEvent(new AccountReplenish(Id, m));
        }

        private static void GuardNegativeMoney(Money m, string msg)
        {
            if (m.IsNegative())
                throw new NegativeMoneyException(msg);
        }

        public void Withdraw(Money m)
        {
            GuardNegativeMoney(m, "Cant withdrawal negative amount of money.");
            if((Amount - m).IsNegative())
                throw new NotEnoughMoneyException("Dont have enough money to pay for bill");

            RaiseEvent(new AccountWithdrawal(Id, m));
        }
    }
}