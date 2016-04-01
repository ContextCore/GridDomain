using System;
using GridDomain.Balance.Domain;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Balance.ReadModel
{
    public class BusinessCurrentBalanceProjectionBuilder : IEventHandler<BalanceReplenishEvent>,
        IEventHandler<BalanceWithdrawalEvent>,
        IEventHandler<BalanceCreatedEvent>
    {
        private readonly Func<BusinessBalanceContext> _contextFactory;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPublisher _publisher;

        public BusinessCurrentBalanceProjectionBuilder(Func<BusinessBalanceContext> contextFactory, IPublisher publisher)
        {
            _publisher = publisher;
            _contextFactory = contextFactory;
        }

        public void Handle(BalanceCreatedEvent msg)
        {
            _logger.Debug("Got event:" + msg.ToPropsString());
            using (var context = _contextFactory())
            {
                //skip creation of existing balance
                if (context.Balances.Find(msg.BalanceId) != null) return;

                var businessCurrentBalance = new BusinessBalance
                {
                    BalanceId = msg.BalanceId,
                    BusinessId = msg.BusinessId
                };

                context.Balances.Add(businessCurrentBalance);
                context.SaveChanges();
                _publisher.Publish(new BalanceCreatedProjectedNotification(msg.BalanceId, msg));
            }
        }

        public void Handle(BalanceReplenishEvent msg)
        {
            HandleChangeEvent(msg, (b, e) => b += e.Amount.Amount);
        }

        public void Handle(BalanceWithdrawalEvent msg)
        {
            HandleChangeEvent(msg, (b, e) => b -= e.Amount.Amount);
        }

        private void HandleChangeEvent(BalanceChangedEvent e,
            Func<decimal, BalanceChangedEvent, decimal> balanceModifier)
        {
            _logger.Debug("Got event:" + e.ToPropsString());
            using (var context = _contextFactory())
            {
                var balance = GetOrCreate(context, e.BalanceId);
                _logger.Debug($"Changing balance {balance.BalanceId} with amount {balance.Amount}");
                _logger.Debug($"by event {e.GetType().Name}, event amount: {e.Amount}");
                balance.Amount = balanceModifier(balance.Amount, e);
                context.SaveChanges();
            }
            _publisher.Publish(new BalanceChangeProjectedNotification(e.BalanceId, e));
        }

        private BusinessBalance GetOrCreate(BusinessBalanceContext context, Guid balanceId)
        {
            var balance = context.Balances.Find(balanceId);
            if (balance != null) return balance;
            //persist unknown balance modification
            Handle(new BalanceCreatedEvent(balanceId, Guid.Empty));
            return context.Balances.Find(balanceId);
        }
    }
}