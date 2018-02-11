using System;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.ProcessManagers.ProcessManagerActorTests
{
    public class TestState : IProcessState
    {
        public TestState(string id, string currentStateName)
        {
            CurrentStateName = currentStateName;
            Id = id;
        }

        public string ProcessingId { get; set; }

        public string Id { get; set; }
        public string CurrentStateName { get; set; }
        public IProcessState Clone()
        {
            return (IProcessState)MemberwiseClone();
        }
    }
}