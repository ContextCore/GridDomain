using GridDomain.CQRS;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Commands
{
    internal class SleepWellCommand: Command
    {
        private GotMoreTiredEvent e;

        public SleepWellCommand(GotMoreTiredEvent e)
        {
            this.e = e;
        }
    }
}