using GridDomain.CQRS;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Commands
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