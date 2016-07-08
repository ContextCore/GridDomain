using GridDomain.CQRS;
using GridDomain.Tests.Sagas.SubscriptionRenew.Events;

namespace GridDomain.Tests.Sagas.SubscriptionRenew.Commands
{
    internal class PayForSubscriptionCommand : Command
    {
        private SubscriptionExpiredEvent e;

        public PayForSubscriptionCommand(SubscriptionExpiredEvent e)
        {
            this.e = e;
        }
    }
}