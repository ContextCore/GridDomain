using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class TestState : ISagaState
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