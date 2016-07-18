using GridDomain.CQRS;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga.Commands
{
    internal class DrinkCupOfCoffeCommand : Command
    {
        private GotTiredEvent e;

        public DrinkCupOfCoffeCommand(GotTiredEvent e)
        {
            this.e = e;
        }
    }
}