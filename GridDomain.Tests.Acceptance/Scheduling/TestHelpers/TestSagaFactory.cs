using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSagaFactory : ISagaFactory<TestSaga, TestSagaStartMessage>,
        ISagaFactory<TestSaga, TestSagaState>,
        IEmptySagaFactory<TestSaga>
    {
        public TestSaga Create(TestSagaStartMessage message)
        {
            return new TestSaga(new TestSagaState(message.SagaId, TestSaga.TestStates.Created));
        }

        public TestSaga Create(TestSagaState state)
        {
            return new TestSaga(state);
        }

        public TestSaga Create()
        {
            return new TestSaga(new TestSagaState(Guid.Empty, TestSaga.TestStates.Created));
        }

    }
}