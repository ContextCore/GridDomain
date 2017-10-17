using System;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.ProcessManagers.ProcessManagerActorTests
{
    public class AsyncLongRunningProcessManagerFactory : IProcessStateFactory<TestState>
    { 
        public TestState Create(object message, TestState state)
        {
            switch (message)
            {
                case BalloonCreated e: return new TestState(e.ProcessId, nameof(AsyncLongRunningProcess.Initial));
            }
            return state;
        }
    }
}