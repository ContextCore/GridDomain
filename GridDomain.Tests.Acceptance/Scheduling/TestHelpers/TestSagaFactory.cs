using System;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSagaFactory : ISagaFactory<TestSaga, TestSagaStartMessage>,
                                   ISagaFactory<TestSaga, TestSagaState>,
                                   ISagaFactory<TestSaga, Guid>
    {
        public TestSaga Create(TestSagaStartMessage message)
        {
            return new TestSaga(new TestSagaState(message.SagaId, TestSaga.TestStates.Created));
        }

        public TestSaga Create(TestSagaState state)
        {
            return new TestSaga(state);
        }

        public TestSaga Create(Guid id)
        {
            return new TestSaga(new TestSagaState(id, TestSaga.TestStates.Created));
        }

    }
}