using System;
using GridDomain.CQRS.Messaging;
using GridDomain.Processes;
using GridDomain.Processes.Creation;
using Serilog;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcessManagerFactory : IProcessManagerCreator<TestProcessState, TestSagaStartMessage>,
                                   IProcessManagerCreator<TestProcessState, Guid>,
                                   IProcessManagerCreator<TestProcessState>
    {
        private readonly ILogger _log;

        public TestProcessManagerFactory(ILogger log)
        {
            _log = log;
        }

        public IProcessManager<TestProcessState> CreateNew(Guid message, Guid? id = null)
        {
            return CreateNew(new TestSagaStartMessage(id ?? Guid.NewGuid(), null, id ?? message));
        }

        public IProcessManager<TestProcessState> Create(TestProcessState message)
        {
            return new ProcessManager<TestProcessState>(new TestSaga(), message, _log);
        }

        public IProcessManager<TestProcessState> CreateNew(TestSagaStartMessage message, Guid? id = null)
        {
            return Create(new TestProcessState(id ?? message.ProcessId, nameof(TestSaga.Initial)));
        }
    }
}