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
        private readonly IReadModelCreator<BusinessBalance> _modelBuilder;
        private readonly IPublisher _publisher;

        public BusinessCurrentBalanceProjectionBuilder(
            IReadModelCreator<BusinessBalance> modelBuilder,
            IPublisher publisher)
        {
            _modelBuilder = modelBuilder;
            _publisher = publisher;
        }

        public void Handle(AccountCreatedEvent e)
        {
            var businessCurrentBalance = new BusinessBalance
            {
                BalanceId = e.BalanceId,
                BusinessId = e.BusinessId
            };

            _modelBuilder.Add(businessCurrentBalance);
            _publisher.Publish(new BalanceCreatedProjectedNotification(e.BalanceId, e));
        }

        public void Handle(AccountBalanceReplenishEvent e)
        {
            _modelBuilder.Modify(e.BalanceId, b => b.Amount += e.Amount.Amount);
            _publisher.Publish(new BalanceChangeProjectedNotification(e.BalanceId));
        }

        public void Handle(PayedForBillEvent e)
        {
            _modelBuilder.Modify(e.BalanceId, b => b.Amount -= e.Amount.Amount);
            _publisher.Publish(new BalanceChangeProjectedNotification(e.BalanceId));
        }
    }
}