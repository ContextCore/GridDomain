using System;
using GridDomain.Processes;

namespace GridDomain.Tests.Unit.Sagas.SagaActorTests
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