using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas.SagaActorTests
{
    public class TestState : ISagaState
    {
        public TestState(Guid id, string currentStateName)
        {
            CurrentStateName = currentStateName;
            Id = id;
        }

        public Guid Id { get; set; }
        public string CurrentStateName { get; set; }
        public Guid ProcessingId { get; set; }
    }
}