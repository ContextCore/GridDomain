using System;
using CommonDomain.Core;
using GridDomain.Balance.Domain.AccountAggregate.Events;
using GridDomain.Logging;
using NLog;
using NMoneys;

namespace GridDomain.Balance.Domain.AccountAggregate
{
    public class Account : AggregateBase
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private Account(Guid id)
        {
            Id = id;
        }

        public Account(Guid id, Guid businessId) : this(id)
        {
            RaiseEvent(new AccountCreatedEvent(id, businessId));
        }

        //Business, campaign or smth else
        public Guid OwnerId { get; private set; }

        public Money Amount { get; private set; }

        private void Apply(AccountCreatedEvent e)
        {
            Id = e.BalanceId;
            OwnerId = e.BusinessId;
        }

        private void Apply(AccountBalanceReplenishEvent e)
        {
            _log.Trace($"Balance {Id} with amount {Amount} increased from event by {e.Amount}");
            Amount += e.Amount;
        }

        private void Apply(PayedForBillEvent e)
        {
            _log.Trace($"Balance {Id} with amount {Amount} decreased from event by {e.Amount}");
            Amount -= e.Amount;
        }

        public void Replenish(Money m)
        {
            _log.Trace($"Balance {Id} with amount {Amount} going to increase from command by {m.Amount}");
            var balanceReplenishEvent = new AccountBalanceReplenishEvent(Id, m);
            GuardNegativeMoney(m, "Cant replenish negative amount of money.", balanceReplenishEvent);
            RaiseEvent(balanceReplenishEvent);
        }

        private static void GuardNegativeMoney(Money m, string msg, object @event)
        {
            if (m.IsNegative())
                throw new NegativeMoneyException(msg + "\r\n" + @event.ToPropsString());
        }

        public void PayBill(Money m, Guid billId)
        {
            var balanceWithdrawalEvent = new PayedForBillEvent(Id, m, billId);
            GuardNegativeMoney(m, "Cant withdrawal negative amount of money.", balanceWithdrawalEvent);
            RaiseEvent(balanceWithdrawalEvent);
        }
    }
}