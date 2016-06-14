using BusinessNews.Domain.AccountAggregate.Events;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.ReadModel;
using NLog;

namespace BusinessNews.ReadModel
{
    //keep in mind 1 instance of projection builder should process only 1 account id 
    public class BusinessCurrentBalanceProjectionBuilder : IEventHandler<AccountBalanceReplenishEvent>,
        IEventHandler<PayedForBillEvent>,
        IEventHandler<AccountCreatedEvent>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IReadModelCreator<BusinessAccount> _modelBuilder;
        private readonly IPublisher _publisher;

        public BusinessCurrentBalanceProjectionBuilder(
            IReadModelCreator<BusinessAccount> modelBuilder,
            IPublisher publisher)
        {
            _modelBuilder = modelBuilder;
            _publisher = publisher;
        }

        public void Handle(AccountBalanceReplenishEvent msg)
        {
            _modelBuilder.Modify(msg.BalanceId, b => b.Amount += msg.Amount.Amount);
            _publisher.Publish(new BalanceChangeProjectedNotification(msg.BalanceId));
        }

        public void Handle(AccountCreatedEvent msg)
        {
            var businessCurrentBalance = new BusinessAccount
            {
                BalanceId = msg.BalanceId,
                BusinessId = msg.BusinessId
            };

            _modelBuilder.Add(businessCurrentBalance);
            _publisher.Publish(new BusinessBalanceCreatedProjectedNotification(msg.BalanceId, msg));
        }

        public void Handle(PayedForBillEvent msg)
        {
            _modelBuilder.Modify(msg.BalanceId, b => b.Amount -= msg.Amount.Amount);
            _publisher.Publish(new BalanceChangeProjectedNotification(msg.BalanceId));
        }
    }
}