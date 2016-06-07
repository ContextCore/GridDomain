using System;
using BusinessNews.Domain.AccountAggregate.Events;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.ReadModel;
using GridDomain.Logging;

namespace BusinessNews.ReadModel
{
    public class TransactionsProjectionBuilder : IEventHandler<AccountBalanceReplenishEvent>,
        IEventHandler<PayedForBillEvent>,
        IEventHandler<AccountCreatedEvent>
    {
        private readonly IReadModelCreator<TransactionHistory> _modelBuilder;

        public TransactionsProjectionBuilder(
            IReadModelCreator<TransactionHistory> modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Handle(AccountBalanceReplenishEvent msg)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = msg.BalanceId,
                EventType = typeof (AccountBalanceReplenishEvent).Name,
                Event = msg.ToPropsString(),
                Id = Guid.NewGuid(),
                TransactionAmount = msg.Amount,
                Time = DateTime.Now
            });
        }

        public void Handle(AccountCreatedEvent msg)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = msg.BalanceId,
                EventType = typeof (AccountCreatedEvent).Name,
                Event = msg.ToPropsString(),
                Id = Guid.NewGuid(),
                Time = DateTime.Now
            });
        }

        public void Handle(PayedForBillEvent msg)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = msg.BalanceId,
                EventType = typeof (PayedForBillEvent).Name,
                Event = msg.ToPropsString(),
                Id = Guid.NewGuid(),
                TransactionAmount = msg.Amount,
                Time = DateTime.Now
            });
        }
    }
}