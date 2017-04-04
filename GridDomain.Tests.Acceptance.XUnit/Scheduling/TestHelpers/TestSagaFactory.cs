using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public class TestSagaFactory : ISagaCreator<TestSagaState, TestSagaStartMessage>,
                                   ISagaCreator<TestSagaState, Guid>,
                                   ISagaCreator<TestSagaState>
    {
        private readonly ILogger _log;

        public TestSagaFactory(ILogger log)
        {
            _log = log;
        }

        public ISaga<TestSagaState> CreateNew(Guid message, Guid? id = null)
        {
            return CreateNew(new TestSagaStartMessage(id ?? Guid.NewGuid(), null, id ?? message));
        }

        public ISaga<TestSagaState> Create(TestSagaState message)
        {
            return new Saga<TestSagaState>(new TestSaga(), message, _log);
        }

        public ISaga<TestSagaState> CreateNew(TestSagaStartMessage message, Guid? id = null)
        {
            return Create(new TestSagaState(id ?? message.SagaId, nameof(TestSaga.Initial)));
        }
    }
}