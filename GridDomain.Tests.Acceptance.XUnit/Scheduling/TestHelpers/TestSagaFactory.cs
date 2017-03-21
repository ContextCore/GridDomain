using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestSagaFactory : ISagaFactory<ISaga<TestSagaState>, TestSagaStartMessage>,
                                   ISagaFactory<ISaga<TestSagaState>, TestSagaState>,
                                   ISagaFactory<ISaga<TestSagaState>, Guid>
    {
        private readonly ILogger _log;

        public TestSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISaga<TestSagaState> Create(Guid message)
        {
            return Create(new TestSagaStartMessage(Guid.NewGuid(), null, message));
        }

        public ISaga<TestSagaState> Create(TestSagaState message)
        {
            return new Saga<TestSagaState>(new TestSaga(),message, _log);
        }

        public ISaga<TestSagaState> Create(TestSagaStartMessage message)
        {
            return Create(new TestSagaState(message.SagaId, nameof(TestSaga.Initial)));
        }
    }
}