using GridDomain.CQRS;
using GridDomain.Tests.Sagas.InstanceSagas.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas.Commands
{
    internal class PayForSubscriptionCommand : Command
    {
        private GotTiredDomainEvent e;

        public PayForSubscriptionCommand(GotTiredDomainEvent e)
        {
            this.e = e;
        }
    }
}