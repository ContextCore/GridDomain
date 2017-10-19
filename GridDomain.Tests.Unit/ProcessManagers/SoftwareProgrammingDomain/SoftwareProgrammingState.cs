using System;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingState : IProcessState
    {
        public SoftwareProgrammingState(Guid id,
                                        string currentStateName,
                                        Guid sofaId = default(Guid),
                                        Guid coffeeMachineId = default(Guid),
                                        Guid personId = default(Guid))
        {
            Id = id;
            SofaId = sofaId;
            CoffeeMachineId = coffeeMachineId;
            CurrentStateName = currentStateName;
            PersonId = personId;
        }

        public Guid PersonId { get; set; }
        public Guid CoffeeMachineId { get; }
        public Guid SofaId { get; set; }
        public Guid BadSleepPersonId { get; set; }
        public Guid Id { get; }
        public string CurrentStateName { get; set; }
        public int CompositeTrack { get; set; }

        public IProcessState Clone()
        {
            return (IProcessState)MemberwiseClone();
        }
    }
}