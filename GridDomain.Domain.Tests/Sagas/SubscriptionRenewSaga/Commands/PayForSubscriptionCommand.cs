using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;

namespace GridDomain.Tests.Sagas.SubscriptionRenewSaga.Commands
{
    internal class PayForSubscriptionCommand
    {
        private SubscriptionExpiredEvent e;

        public PayForSubscriptionCommand(SubscriptionExpiredEvent e)
        {
            this.e = e;
        }
    }
}