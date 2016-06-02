using System;
using GridDomain.Balance.Domain.AccountAggregate.Events;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.ReadModel;
using GridDomain.Logging;

namespace GridDomain.Balance.ReadModel
{
    public class TransactionsProjectionBuilder : IEventHandler<AccountBalanceReplenishEvent>,
        IEventHandler<AccountWithdrawalEvent>,
        IEventHandler<AccountCreatedEvent>
    {
        private readonly IReadModelCreator<TransactionHistory> _modelBuilder;

        public TransactionsProjectionBuilder(
            IReadModelCreator<TransactionHistory> modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Handle(AccountCreatedEvent e)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = e.BalanceId,
                EventType = typeof (AccountCreatedEvent).Name,
                Event = e.ToPropsString(),
                Id = Guid.NewGuid(),
                Time = DateTime.Now
            });
        }

        public void Handle(AccountBalanceReplenishEvent e)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = e.BalanceId,
                EventType = typeof (AccountBalanceReplenishEvent).Name,
                Event = e.ToPropsString(),
                Id = Guid.NewGuid(),
                TransactionAmount = e.Amount,
                Time = DateTime.Now
            });
        }

        public void Handle(AccountWithdrawalEvent e)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = e.BalanceId,
                EventType = typeof (AccountWithdrawalEvent).Name,
                Event = e.ToPropsString(),
                Id = Guid.NewGuid(),
                TransactionAmount = e.Amount,
                Time = DateTime.Now
            });
        }
    }
}