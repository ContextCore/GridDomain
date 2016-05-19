using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;

namespace GridDomain.Tests.Sagas.SubscriptionRenewSaga.Commands
{
    class ChangeSubscriptionCommand
    {
        private NotEnoughFondsFailure e;

        public ChangeSubscriptionCommand(NotEnoughFondsFailure e)
        {
            this.e = e;
        }
    }
}