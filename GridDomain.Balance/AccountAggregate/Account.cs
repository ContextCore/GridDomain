using System;
using BusinessNews.Domain.AccountAggregate.Events;
using CommonDomain.Core;
using GridDomain.Logging;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate
{
    public class Account : AggregateBase
    {
        private readonly ISoloLogger _log = LogManager.GetLogger();

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
            GuardNegativeMoney(m, "Cant replenish negative amount of money.");
            _log.Trace($"Balance {Id} with amount {Amount} going to increase from command by {m.Amount}");
            var balanceReplenishEvent = new AccountBalanceReplenishEvent(Id, m);
            RaiseEvent(balanceReplenishEvent);
        }

        private static void GuardNegativeMoney(Money m, string msg)
        {
            if (m.IsNegative())
                throw new NegativeMoneyException(msg);
        }

        public void PayBill(Money m, Guid billId)
        {
            //TODO: replace with Apply logic reuse ? need to peek the changes in apply ? 
            GuardNegativeMoney(m, "Cant withdrawal negative amount of money.");
            GuardNegativeMoney(Amount - m, "Dont have enought money to pay for bill");
            var balanceWithdrawalEvent = new PayedForBillEvent(Id, m, billId);
            RaiseEvent(balanceWithdrawalEvent);
        }
    }
}