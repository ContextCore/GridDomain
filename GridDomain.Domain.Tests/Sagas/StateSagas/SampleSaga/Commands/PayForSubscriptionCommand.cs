using GridDomain.CQRS;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Commands
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