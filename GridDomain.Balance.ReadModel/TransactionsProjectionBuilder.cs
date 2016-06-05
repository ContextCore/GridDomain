using System;
using GridDomain.Balance.Domain.AccountAggregate.Events;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.ReadModel;
using GridDomain.Logging;

namespace GridDomain.Balance.ReadModel
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

        public void Handle(AccountCreatedEvent cmd)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = cmd.BalanceId,
                EventType = typeof (AccountCreatedEvent).Name,
                Event = cmd.ToPropsString(),
                Id = Guid.NewGuid(),
                Time = DateTime.Now
            });
        }

        public void Handle(AccountBalanceReplenishEvent cmd)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = cmd.BalanceId,
                EventType = typeof (AccountBalanceReplenishEvent).Name,
                Event = cmd.ToPropsString(),
                Id = Guid.NewGuid(),
                TransactionAmount = cmd.Amount,
                Time = DateTime.Now
            });
        }

        public void Handle(PayedForBillEvent cmd)
        {
            _modelBuilder.Add(new TransactionHistory
            {
                BalanceId = cmd.BalanceId,
                EventType = typeof (PayedForBillEvent).Name,
                Event = cmd.ToPropsString(),
                Id = Guid.NewGuid(),
                TransactionAmount = cmd.Amount,
                Time = DateTime.Now
            });
        }
    }
}