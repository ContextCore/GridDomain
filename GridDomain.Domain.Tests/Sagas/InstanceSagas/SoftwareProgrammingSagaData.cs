using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class SoftwareProgrammingSagaData: ISagaState<State>
    {
        public Guid PersonId { get; set; }
        public State CurrentState { get; set; }
        public Guid CoffeeMachineId { get; }
        public Guid SofaId { get; }
        public SoftwareProgrammingSagaData(State currentState, Guid sofaId = default(Guid), Guid coffeeMachineId = default(Guid))
        {
            CurrentState = currentState;
            SofaId = sofaId;
            CoffeeMachineId = coffeeMachineId;
        }
    }
}