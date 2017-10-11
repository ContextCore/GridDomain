using System;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Serilog;

namespace GridDomain.Tests.Unit.ProcessManagers.ProcessManagerActorTests
{
    public class AsyncLongRunningProcessManagerFactory : IProcessManagerCreator<TestState>,
                                               IProcessManagerCreator<TestState, BalloonCreated>
    {
        private readonly ILogger _log;

        public AsyncLongRunningProcessManagerFactory(ILogger log)
        {
            _log = log;
        }

        public IProcessInstance<TestState> Create(TestState message)
        {
            return new ProcessManager<TestState>(new AsyncLongRunningProcess(), message, _log);
        }

        public IProcessInstance<TestState> CreateNew(BalloonCreated message, Guid? processId = null)
        {
            return Create(new TestState(processId ?? message.ProcessId, nameof(AsyncLongRunningProcess.Initial)));
        }
    }
}