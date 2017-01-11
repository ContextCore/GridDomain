using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestSagaFactory : ISagaFactory<ISagaInstance<TestSaga,TestSagaState>, TestSagaStartMessage>,
                                   ISagaFactory<ISagaInstance<TestSaga,TestSagaState>, SagaDataAggregate<TestSagaState>>,
                                   ISagaFactory<ISagaInstance<TestSaga, TestSagaState>,  Guid>
    {
        //public TestSaga Create(TestSagaStartMessage message)
        //{
        //    return new TestSaga(new TestSagaState(message.SagaId, TestSaga.TestStates.Created));
        //}
        //
        //public TestSaga Create(TestSagaState state)
        //{
        //    return new TestSaga(state);
        //}
        //
        //public TestSaga Create(Guid id)
        //{
        //    return new TestSaga(new TestSagaState(id, TestSaga.TestStates.Created));
        //}


        public ISagaInstance<TestSaga, TestSagaState> Create(TestSagaStartMessage message)
        {
            return
                Create(new SagaDataAggregate<TestSagaState>(message.SagaId, new TestSagaState(nameof(TestSaga.Initial))));
        }

        public ISagaInstance<TestSaga, TestSagaState> Create(SagaDataAggregate<TestSagaState> message)
        {
            return SagaInstance.New(new TestSaga(), message);
        }

        public ISagaInstance<TestSaga, TestSagaState> Create(Guid message)
        {
            return Create(new TestSagaStartMessage(Guid.NewGuid(), null, message));
        }
    }
}