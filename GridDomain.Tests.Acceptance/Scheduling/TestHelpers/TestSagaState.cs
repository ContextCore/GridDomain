using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSagaState : SagaStateAggregate<TestSaga.TestStates, TestSaga.Transitions>
    {
        public TestSagaState(Guid id) : base(id)
        {
        }

        public TestSagaState(Guid id, TestSaga.TestStates state) : base(id, state)
        {
        }
    }
}