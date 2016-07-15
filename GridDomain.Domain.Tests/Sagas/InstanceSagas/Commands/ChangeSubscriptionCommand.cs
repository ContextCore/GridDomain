using GridDomain.CQRS;
using GridDomain.Tests.Sagas.InstanceSagas.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas.Commands
{
    internal class ChangeSubscriptionCommand: Command
    {
        private FeltMoreTiredDomainEvent e;

        public ChangeSubscriptionCommand(FeltMoreTiredDomainEvent e)
        {
            this.e = e;
        }
    }
}