using GridDomain.CQRS;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Commands
{
    internal class SleepWellCommand: Command
    {
        private CoffeMakeFailedEvent e;

        public SleepWellCommand(CoffeMakeFailedEvent e)
        {
            this.e = e;
        }
    }
}