using GridDomain.CQRS;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;

namespace GridDomain.Tests.Sagas.SubscriptionRenewSaga.Commands
{
    internal class ChangeSubscriptionCommand: Command
    {
        private NotEnoughFundsFailure e;

        public ChangeSubscriptionCommand(NotEnoughFundsFailure e)
        {
            this.e = e;
        }
    }
}