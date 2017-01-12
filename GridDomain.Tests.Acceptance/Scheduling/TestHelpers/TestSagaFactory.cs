using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSagaFactory : ISagaFactory<ISagaInstance<TestSaga,TestSagaState>, TestSagaStartMessage>,
                                   ISagaFactory<ISagaInstance<TestSaga,TestSagaState>, SagaStateAggregate<TestSagaState>>,
                                   ISagaFactory<ISagaInstance<TestSaga, TestSagaState>,  Guid>
    {
        public ISagaInstance<TestSaga, TestSagaState> Create(TestSagaStartMessage message)
        {
            return
                Create(new SagaStateAggregate<TestSagaState>(new TestSagaState(message.SagaId,nameof(TestSaga.Initial))));
        }

        public ISagaInstance<TestSaga, TestSagaState> Create(SagaStateAggregate<TestSagaState> message)
        {
            return SagaInstance.New(new TestSaga(), message);
        }

        public ISagaInstance<TestSaga, TestSagaState> Create(Guid message)
        {
            return Create(new TestSagaStartMessage(Guid.NewGuid(), null, message));
        }
    }
}