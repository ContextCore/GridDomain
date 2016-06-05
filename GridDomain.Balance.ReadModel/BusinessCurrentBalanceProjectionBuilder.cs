using GridDomain.Balance.Domain.AccountAggregate.Events;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.ReadModel;
using NLog;

namespace GridDomain.Balance.ReadModel
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

        public void Handle(AccountCreatedEvent cmd)
        {
            var businessCurrentBalance = new BusinessBalance
            {
                BalanceId = cmd.BalanceId,
                BusinessId = cmd.BusinessId
            };

            _modelBuilder.Add(businessCurrentBalance);
            _publisher.Publish(new BalanceCreatedProjectedNotification(cmd.BalanceId, cmd));
        }

        public void Handle(AccountBalanceReplenishEvent cmd)
        {
            _modelBuilder.Modify(cmd.BalanceId, b => b.Amount += cmd.Amount.Amount);
            _publisher.Publish(new BalanceChangeProjectedNotification(cmd.BalanceId));
        }

        public void Handle(PayedForBillEvent cmd)
        {
            _modelBuilder.Modify(cmd.BalanceId, b => b.Amount -= cmd.Amount.Amount);
            _publisher.Publish(new BalanceChangeProjectedNotification(cmd.BalanceId));
        }
    }
}