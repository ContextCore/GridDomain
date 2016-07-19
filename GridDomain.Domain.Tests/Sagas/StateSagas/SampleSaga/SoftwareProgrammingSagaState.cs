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

        public void RememberEvent(GotTiredEvent e)
        {
            RaiseEvent(new GotTiredSagaEvent(e));
        }

        private void Apply(GotTiredSagaEvent e)
        {
            SourceId = e.SourceId;
        }

        public Guid SourceId { get; private set; }
    }

    public class GotTiredSagaEvent : GotTiredEvent
    {
        public GotTiredSagaEvent(GotTiredEvent e) : base(e.SourceId)
        {
        }
    }
}