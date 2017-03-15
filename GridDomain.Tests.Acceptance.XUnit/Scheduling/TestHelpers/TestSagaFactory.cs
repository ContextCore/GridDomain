using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestSagaFactory : ISagaFactory<ISaga<TestSaga, TestSagaState>, TestSagaStartMessage>,
                                   ISagaFactory<ISaga<TestSaga, TestSagaState>, SagaStateAggregate<TestSagaState>>,
                                   ISagaFactory<ISaga<TestSaga, TestSagaState>, Guid>
    {
        private readonly ILogger _log;

        public TestSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISaga<TestSaga, TestSagaState> Create(Guid message)
        {
            return Create(new TestSagaStartMessage(Guid.NewGuid(), null, message));
        }

        public ISaga<TestSaga, TestSagaState> Create(SagaStateAggregate<TestSagaState> message)
        {
            return Saga.New(new TestSaga(), message, _log);
        }

        public ISaga<TestSaga, TestSagaState> Create(TestSagaStartMessage message)
        {
            return Create(new SagaStateAggregate<TestSagaState>(new TestSagaState(message.SagaId, nameof(TestSaga.Initial))));
        }
    }
}