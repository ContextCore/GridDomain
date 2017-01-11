using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    public class TestState : ISagaState
    {
        public TestState(string currentStateName)
        {
            CurrentStateName = currentStateName;
        }
        public string CurrentStateName { get; set; }
        public Guid ProcessingId { get; set; }
    }
}