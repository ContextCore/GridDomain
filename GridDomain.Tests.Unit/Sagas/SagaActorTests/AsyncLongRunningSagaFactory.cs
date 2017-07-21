using System;
using GridDomain.Processes;
using GridDomain.Processes.Creation;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Serilog;

namespace GridDomain.Tests.Unit.Sagas.SagaActorTests
{
    public class AsyncLongRunningProcessManagerFactory : IProcessManagerCreator<TestState>,
                                               IProcessManagerCreator<TestState, BalloonCreated>
    {
        private readonly ILogger _log;

        public AsyncLongRunningProcessManagerFactory(ILogger log)
        {
            _log = log;
        }

        public IProcessManager<TestState> Create(TestState message)
        {
            return new ProcessManager<TestState>(new AsyncLongRunningProcess(), message, _log);
        }

        public IProcessManager<TestState> CreateNew(BalloonCreated message, Guid? id = null)
        {
            return Create(new TestState(id ?? message.ProcessId, nameof(AsyncLongRunningProcess.Initial)));
        }
    }
}