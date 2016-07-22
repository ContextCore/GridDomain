using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class GotMoreTiredSagaEvent : GotMoreTiredEvent
    {
        public GotMoreTiredSagaEvent(GotMoreTiredEvent e) : base(e.SourceId)
        {
        }
    }
}