using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class SoftwareProgrammingSagaState :
        SagaStateAggregate<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>
    {
        public SoftwareProgrammingSagaState(Guid id) : base(id)
        {
        }

        public SoftwareProgrammingSagaState(Guid id, SoftwareProgrammingSaga.States state) : base(id, state)
        {
        }

        public void RememberEvent(GotMoreTiredEvent e)
        {
            RaiseEvent(new GotMoreTiredSagaEvent(e));
        }

        private void Apply(GotMoreTiredSagaEvent e)
        {
            SourceId = e.SourceId;
        }

        public Guid SourceId { get; private set; }
    }

    public class GotMoreTiredSagaEvent : GotMoreTiredEvent
    {
        public GotMoreTiredSagaEvent(GotMoreTiredEvent e) : base(e.SourceId)
        {
        }
    }
}