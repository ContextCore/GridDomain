using GridDomain.CQRS;
using GridDomain.Tests.Sagas.SubscriptionRenew.Events;

namespace GridDomain.Tests.Sagas.SubscriptionRenew.Commands
{
    internal class ChangeSubscriptionCommand: Command
    {
        private NotEnoughFondsFailure e;

        public ChangeSubscriptionCommand(NotEnoughFondsFailure e)
        {
            this.e = e;
        }
    }
}