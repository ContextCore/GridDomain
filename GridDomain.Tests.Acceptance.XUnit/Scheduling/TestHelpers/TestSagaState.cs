using System;
using System.Runtime.Serialization;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestSagaState : ISagaState
    {
        public TestSagaState(Guid id, string currentStateName)
        {
            CurrentStateName = currentStateName;
            Id = id;
        }

        public Guid Id { get; set; }
        public string CurrentStateName { get; set; }

        public object Clone()
        {
           return MemberwiseClone();
        }
    }
}