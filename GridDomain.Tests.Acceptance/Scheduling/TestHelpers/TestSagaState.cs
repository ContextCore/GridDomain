using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSagaState : ISagaState
    {
        public TestSagaState(string currentStateName)
        {
            CurrentStateName = currentStateName;
        }

        public string CurrentStateName { get; set; }
    }
}