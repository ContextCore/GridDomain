using System;
using CommonDomain;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.EventSourcing.Sagas.StateSagas;
using GridDomain.Tests.Framework;

namespace GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga
{
    public class SoftwareProgrammingSagaState:SagaStateAggregate<SoftwareProgrammingSaga.States, SoftwareProgrammingSaga.Triggers>
    {

        class Snapshot : IMemento
        {
            public Guid Id { get; set; }
            public int Version { get; set; }
            public Guid CoffeMachineId { get; set; }
            public Guid PersonId { get; set; }
            public SoftwareProgrammingSaga.States State { get; set; }

        }

        protected override IMemento GetSnapshot()
        {
            return new Snapshot() { CoffeMachineId =CoffeMachineId, Id = Id, PersonId = PersonId, Version = Version, State = MachineState };
        }

        public static SoftwareProgrammingSagaState FromSnapshot(IMemento m)
        {
            Snapshot s = m as Snapshot;
            if(s == null)
                throw new WrongSnapshotTypeReceivedException(m.GetType(),typeof(Snapshot));
            var aggregate = new SoftwareProgrammingSagaState(s.Id,s.State);

            aggregate.RememberPerson(s.PersonId);
            aggregate.RememberBadCoffeMachine(s.CoffeMachineId);
            aggregate.Version = s.Version;
            aggregate.ClearEvents();
            return aggregate;
        }

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