using CommonDomain.Persistence;
using GridDomain.CQRS;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    public class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand>
    {
        private readonly IRepository _repo;

        public CreateSubscriptionCommandHandler(IRepository repo)
        {
            _repo = repo;
        }

        public void Handle(CreateSubscriptionCommand cmd)
        {
            var serviceSubscription = new Subscription(cmd.SubscriptionId, cmd.Period, cmd.Cost, cmd.Name,
                cmd.Grants);
            _repo.Save(serviceSubscription, cmd.Id);
        }
    }
}