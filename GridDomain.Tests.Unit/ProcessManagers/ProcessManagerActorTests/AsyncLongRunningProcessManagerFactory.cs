using System;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;

namespace GridDomain.Tests.Unit.ProcessManagers.ProcessManagerActorTests
{
    public class AsyncLongRunningProcessManagerFactory : IProcessStateFactory<TestState>
    { 
        public TestState Create(object message)
        {
            switch (message)
            {
                case BalloonCreated e: return new TestState(e.ProcessId, nameof(AsyncLongRunningProcess.Initial));
            }
            throw new CannotCreateStateFromMessageException(message);

        }
    }
}