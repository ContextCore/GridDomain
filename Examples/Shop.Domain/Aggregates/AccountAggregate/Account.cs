using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using NMoneys;
using Shop.Domain.Aggregates.AccountAggregate.Events;

namespace Shop.Domain.Aggregates.AccountAggregate
{
    public class Account : Aggregate
    {
        private Account(Guid id) : base(id) {}

        public Account(Guid id, Guid userId, int number) : this(id)
        {
            Produce(new AccountCreated(id, userId, number));
        }

        public Guid UserId { get; private set; }
        public Money Amount { get; private set; }
        public int Number { get; private set; }

        private void Apply(AccountCreated e)
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

        public void Replenish(Money m, Guid replenishSource)
        {
            GuardNegativeMoney(m, "Cant replenish negative amount of money.");
            Produce(new AccountReplenish(Id, replenishSource, m));
        }

        private static void GuardNegativeMoney(Money m, string msg)
        {
            if (m.IsNegative())
                throw new NegativeMoneyException(msg);
        }

        public void Withdraw(Money m, Guid withdrawSource)
        {
            GuardNegativeMoney(m, "Cant withdrawal negative amount of money.");
            if ((Amount - m).IsNegative())
                throw new NotEnoughMoneyException("Dont have enough money to pay for bill");

            Produce(new AccountWithdrawal(Id, withdrawSource, m));
        }
    }
}