using CommonDomain.Persistence;
using GridDomain.CQRS;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    public class CreateServiceSubscriptionCommandHandler : ICommandHandler<CreateServiceSubscriptionCommand>
    {
        private readonly IRepository _repo;

        public CreateServiceSubscriptionCommandHandler(IRepository repo)
        {
            _repo = repo;
        }

        public void Handle(CreateServiceSubscriptionCommand cmd)
        {
            var serviceSubscription = new ServiceSubscription(cmd.SubscriptionId,cmd.Period, cmd.Cost, cmd.Name, cmd.Grants);
            _repo.Save(serviceSubscription, cmd.Id);
        }
    }
}