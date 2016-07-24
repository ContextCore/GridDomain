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
        private void Apply(PersonRememberedEvent e)
        {
            PersonId = e.PersonId;
        }

        public Guid SourceId { get; private set; }
        public Guid CoffeMachineId { get; private set; }
        public Guid PersonId { get; private set; }

        //  public Guid FavoritSofaId { get; private set; }
        public void RememberPerson(Guid personId)
        {
            RaiseEvent(new PersonRememberedEvent(Id,personId));
        }
    }
}