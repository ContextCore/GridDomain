using GridDomain.CQRS;
using GridDomain.Tests.Sagas.InstanceSagas.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas.Commands
{
    internal class GoSleepCommand: Command
    {
        private FeltMoreTiredDomainEvent e;

        public GoSleepCommand(FeltMoreTiredDomainEvent e)
        {
            this.e = e;
        }
    }
}