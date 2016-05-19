using System.Linq;
using GridDomain.Balance.Domain;
using GridDomain.Balance.Domain.BalanceAggregate.Events;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.ReadModel;
using GridDomain.Logging;
using NLog;

namespace GridDomain.Balance.ReadModel
{
    //keep in mind 1 instance of projection builder should process only 1 balance id 
    public class BusinessCurrentBalanceProjectionBuilder : IEventHandler<BalanceReplenishEvent>,
                                                           IEventHandler<BalanceWithdrawalEvent>,
                                                           IEventHandler<BalanceCreatedEvent>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPublisher _publisher;
        private readonly IReadModelCreator<BusinessBalance> _modelBuilder;

        public BusinessCurrentBalanceProjectionBuilder(
                      IReadModelCreator<BusinessBalance> modelBuilder,
                      IPublisher publisher)
        {
            _modelBuilder = modelBuilder;
            _publisher = publisher;
        }

        public void Handle(BalanceReplenishEvent e)
        {
            _modelBuilder.Modify(e.BalanceId, b => b.Amount += e.Amount.Amount);
            _publisher.Publish(new BalanceChangeProjectedNotification(e.BalanceId, e));
        }

        public void Handle(BalanceWithdrawalEvent e)
        {
            _modelBuilder.Modify(e.BalanceId, b => b.Amount -= e.Amount.Amount);
            _publisher.Publish(new BalanceChangeProjectedNotification(e.BalanceId, e));
        }

        public void Handle(BalanceCreatedEvent e)
        {
            var businessCurrentBalance = new BusinessBalance()
            {
                BalanceId = e.BalanceId,
                BusinessId = e.BusinessId
            };

            _modelBuilder.Add(businessCurrentBalance);
            _publisher.Publish(new BalanceCreatedProjectedNotification(e.BalanceId, e));
        }
    }
}
