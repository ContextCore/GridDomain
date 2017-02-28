using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestSagaFactory : ISagaFactory<ISagaInstance<TestSaga, TestSagaState>, TestSagaStartMessage>,
                                   ISagaFactory<ISagaInstance<TestSaga, TestSagaState>, SagaStateAggregate<TestSagaState>>,
                                   ISagaFactory<ISagaInstance<TestSaga, TestSagaState>, Guid>
    {
        private readonly ILogger _log;

        public TestSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISagaInstance<TestSaga, TestSagaState> Create(Guid message)
        {
            return Create(new TestSagaStartMessage(Guid.NewGuid(), null, message));
        }

        public ISagaInstance<TestSaga, TestSagaState> Create(SagaStateAggregate<TestSagaState> message)
        {
            return SagaInstance.New(new TestSaga(), message, _log);
        }

        public ISagaInstance<TestSaga, TestSagaState> Create(TestSagaStartMessage message)
        {
            return Create(new SagaStateAggregate<TestSagaState>(new TestSagaState(message.SagaId, nameof(TestSaga.Initial))));
        }
    }
}