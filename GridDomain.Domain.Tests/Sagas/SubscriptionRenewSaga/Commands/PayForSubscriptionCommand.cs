using GridDomain.CQRS;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;

namespace GridDomain.Tests.Sagas.SubscriptionRenewSaga.Commands
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