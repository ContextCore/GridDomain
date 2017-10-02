using System;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcessState : IProcessState
    {
        public TestProcessState(Guid id, string currentStateName)
        {
            CurrentStateName = currentStateName;
            Id = id;
        }

        public Guid Id { get; set; }
        public string CurrentStateName { get; set; }

        public IProcessState Clone()
        {
           return (IProcessState)MemberwiseClone();
        }
    }
}