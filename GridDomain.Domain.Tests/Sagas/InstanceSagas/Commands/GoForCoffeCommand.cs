using GridDomain.CQRS;
using GridDomain.Tests.Sagas.InstanceSagas.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas.Commands
{
    internal class GoForCoffeCommand : Command
    {
        private GotTiredDomainEvent e;

        public GoForCoffeCommand(GotTiredDomainEvent e)
        {
            this.e = e;
        }
    }
}