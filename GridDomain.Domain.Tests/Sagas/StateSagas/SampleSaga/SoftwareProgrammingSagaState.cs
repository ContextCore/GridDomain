using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using Newtonsoft.Json;

namespace GridDomain.Tests.Sagas.StateSagas.SampleSaga
{
    public class SoftwareProgrammingSagaState:SagaStateAggregate<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>
    {
        private SoftwareProgrammingSagaState(Guid id) : base(id)
        {
        }

        public SoftwareProgrammingSagaState(Guid id, SoftwareProgrammingSaga.States state) : base(id, state)
        {
        }

        public void RememberBadCoffeMachine(Guid machineId)
        {
            RaiseEvent(new BadCoffeMachineRememberedEvent(Id,machineId));
        }

        private void Apply(BadCoffeMachineRememberedEvent e)
        {
            CoffeMachineId = e.CoffeMachineId;
        }
        private void Apply(PersonRememberedEvent e)
        {
            PersonId = e.PersonId;
        }
        public Guid CoffeMachineId { get; private set; }
        public Guid PersonId { get; private set; }

        public void RememberPerson(Guid personId)
        {
            RaiseEvent(new PersonRememberedEvent(Id,personId));
        }
    }
}