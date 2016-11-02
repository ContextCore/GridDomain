using System;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NMoneys;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class SoftwareProgrammingSagaData : ISagaState
    {
        public Guid PersonId { get; set; }
        public string CurrentStateName { get; set; }
        public Guid CoffeeMachineId { get; }
        public Guid SofaId { get; set; }

        public SoftwareProgrammingSagaData(string stateName,
                                           Guid sofaId = default(Guid),
                                           Guid coffeeMachineId = default(Guid))
        {
            SofaId = sofaId;
            CoffeeMachineId = coffeeMachineId;
            CurrentStateName = stateName;
        }
    }
}