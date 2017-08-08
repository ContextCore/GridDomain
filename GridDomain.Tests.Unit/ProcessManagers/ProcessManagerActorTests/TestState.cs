using System;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.ProcessManagers.ProcessManagerActorTests
{
    public class TestState : IProcessState
    {
        public TestState(Guid id, string currentStateName)
        {
            CurrentStateName = currentStateName;
            Id = id;
        }

        public Guid ProcessingId { get; set; }

        public Guid Id { get; set; }
        public string CurrentStateName { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}