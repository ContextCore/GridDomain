using System;
using CommonDomain.Core;
using GridDomain.Logging;
using NMoneys;

namespace GridDomain.Balance.Domain
{
    public class Balance:AggregateBase
    {
        //Business, campaign or smth else
        public Guid OwnerId { get; private set; }

        public Money Amount { get; private set; } 


        private Balance(Guid id)
        {
            Id = id;
        }

        public Balance(Guid id, Guid businessId):this(id)
        {
            RaiseEvent(new BalanceCreatedEvent(id, businessId));
        }

        private void Apply(BalanceCreatedEvent e)
        {
            Id = e.BalanceId;
            OwnerId = e.BusinessId;
        }
        private void Apply(BalanceReplenishEvent e)
        {
            Amount += e.Amount;
        }

        private void Apply(BalanceWithdrawalEvent e)
        {
            Amount -= e.Amount;
        }

        public void Replenish(Money m)
        {
          
            var balanceReplenishEvent = new BalanceReplenishEvent(Id, m);
            GuardNegativeMoney(m, "Cant replenish negative amount of money.", balanceReplenishEvent);

            RaiseEvent(balanceReplenishEvent);
        }

        private static void GuardNegativeMoney(Money m, string msg, object @event)
        {
            if (m.IsNegative())
                throw new NegativeMoneyException(msg + "\r\n" + @event.ToPropsString());
        }

        public void Withdrawal(Money m)
        {
            var balanceWithdrawalEvent = new BalanceWithdrawalEvent(Id, m);
            GuardNegativeMoney(m, "Cant withdrawal negative amount of money.", balanceWithdrawalEvent);
            RaiseEvent(balanceWithdrawalEvent);
        }
    }
}