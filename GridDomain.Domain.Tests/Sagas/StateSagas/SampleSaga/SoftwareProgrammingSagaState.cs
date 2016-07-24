using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;

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

        public void RememberEvent(CoffeMakeFailedEvent e)
        {
            RaiseEvent(new CoffeMakeFailedRememberedEvent(Id,e));
        }

        private void Apply(CoffeMakeFailedRememberedEvent e)
        {
            SourceId = e.SourceId;
        }

        public Guid SourceId { get; private set; }
        public Guid CoffeMachineId { get; private set; }
      //  public Guid FavoritSofaId { get; private set; }
    }
}