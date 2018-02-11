using System;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain
{
    public class SoftwareProgrammingState : IProcessState
    {
        public SoftwareProgrammingState(string id,
                                        string currentStateName,
                                        string sofaId = null,
                                        string coffeeMachineId = null,
                                        string personId = null)
        {
            Id = id;
            SofaId = sofaId;
            CoffeeMachineId = coffeeMachineId;
            CurrentStateName = currentStateName;
            PersonId = personId;
        }

        public string PersonId { get; set; }
        public string CoffeeMachineId { get; }
        public string SofaId { get; set; }
        public string BadSleepPersonId { get; set; }
        public string Id { get; }
        public string CurrentStateName { get; set; }
        public int CompositeTrack { get; set; }

        public IProcessState Clone()
        {
            return (IProcessState)MemberwiseClone();
        }
    }
}