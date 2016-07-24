using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NMoneys;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    class SoftwareProgrammingSagaData : ISagaState<State>
    {
        public Guid PersonId { get; set; }
        public string CurrentState { get; set; }
        public Guid CoffeeMachineId { get; }
        public Guid SofaId { get; }
       // public Money Price { get; set; }

        public SoftwareProgrammingSagaData(string state,
                                           Guid sofaId = default(Guid),
                                           Guid coffeeMachineId = default(Guid))
        {
            SofaId = sofaId;
            CoffeeMachineId = coffeeMachineId;
            CurrentState = state;
        }

        public SoftwareProgrammingSagaData(State state,
                                         Guid sofaId = default(Guid),
                                         Guid coffeeMachineId = default(Guid))
        {
            SofaId = sofaId;
            CoffeeMachineId = coffeeMachineId;
            CurrentState = state.Name;
        }
    }
}