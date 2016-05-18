using System;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.Logging;
using NLog;
using NMoneys;

namespace GridDomain.Balance.Domain
{
    public class MoneyBalance:AggregateBase
    {
        //Business, campaign or smth else
        public Guid OwnerId { get; private set; }

        public Money Amount { get; private set; }

        private Logger _log = LogManager.GetCurrentClassLogger();

        private MoneyBalance(Guid id)
        {
            Id = id;
        }

        public MoneyBalance(Guid id, Guid businessId):this(id)
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
            _log.Trace($"Balance {Id} with amount {Amount} increased from event by {e.Amount}");
            Amount += e.Amount;
        }

        private void Apply(BalanceWithdrawalEvent e)
        {
            _log.Trace($"Balance {Id} with amount {Amount} decreased from event by {e.Amount}");
            Amount -= e.Amount;
        }

        public void Replenish(Money m)
        {
            _log.Trace($"Balance {Id} with amount {Amount} going to increase from command by {m.Amount}");
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
            _log.Trace($"Balance {Id} with amount {Amount} going to decrease from command by {m.Amount}");
            var balanceWithdrawalEvent = new BalanceWithdrawalEvent(Id, m);
            GuardNegativeMoney(m, "Cant withdrawal negative amount of money.", balanceWithdrawalEvent);
            RaiseEvent(balanceWithdrawalEvent);
        }
    }
}