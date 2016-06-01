using System;
using GridDomain.Balance.Domain.BalanceAggregate.Events;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.ReadModel;
using GridDomain.Logging;

namespace GridDomain.Balance.ReadModel
{
    public class TransactionsProjectionBuilder : IEventHandler<BalanceReplenishEvent>,
        IEventHandler<BalanceWithdrawalEvent>,
        IEventHandler<BalanceCreatedEvent>
    {
        private readonly IReadModelCreator<TransactionHistory> _modelBuilder;

        public TransactionsProjectionBuilder(
            IReadModelCreator<TransactionHistory> modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Handle(BalanceReplenishEvent e)
        {
            _modelBuilder.Add(new TransactionHistory()
            { 
                BalanceId  = e.BalanceId,
                EventType = typeof(BalanceReplenishEvent).Name,
                Event = e.ToPropsString(),
                Id = Guid.NewGuid(),
                TransactionAmount = e.Amount,
                Time = DateTime.Now
            });
        }

        public void Handle(BalanceWithdrawalEvent e)
        {
            _modelBuilder.Add(new TransactionHistory()
            {
                BalanceId = e.BalanceId,
                EventType = typeof(BalanceWithdrawalEvent).Name,
                Event = e.ToPropsString(),
                Id = Guid.NewGuid(),
                TransactionAmount = e.Amount,
                Time = DateTime.Now
            });
        }

        public void Handle(BalanceCreatedEvent e)
        {
            _modelBuilder.Add(new TransactionHistory()
            {
                BalanceId = e.BalanceId,
                EventType = typeof(BalanceCreatedEvent).Name,
                Event = e.ToPropsString(),
                Id = Guid.NewGuid(),
                Time = DateTime.Now
            });
        }
    }
}