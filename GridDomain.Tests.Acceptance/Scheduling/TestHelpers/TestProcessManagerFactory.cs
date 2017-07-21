using System;
using GridDomain.CQRS.Messaging;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using Serilog;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcessManagerFactory : IProcessManagerCreator<TestProcessState, TestProcessStartMessage>,
                                   IProcessManagerCreator<TestProcessState, Guid>,
                                   IProcessManagerCreator<TestProcessState>
    {
        private readonly ILogger _log;

        public TestProcessManagerFactory(ILogger log)
        {
            _log = log;
        }

        public IProcessManager<TestProcessState> CreateNew(Guid message, Guid? processId = null)
        {
            return CreateNew(new TestProcessStartMessage(processId ?? Guid.NewGuid(), null, processId ?? message));
        }

        public IProcessManager<TestProcessState> Create(TestProcessState message)
        {
            return new ProcessManager<TestProcessState>(new TestProcess(), message, _log);
        }

        public IProcessManager<TestProcessState> CreateNew(TestProcessStartMessage message, Guid? processId = null)
        {
            return Create(new TestProcessState(processId ?? message.ProcessId, nameof(TestProcess.Initial)));
        }
    }
}