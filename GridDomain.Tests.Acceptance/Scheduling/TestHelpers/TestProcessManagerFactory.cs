using System;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcessManagerFactory : IProcessStateFactory<TestProcessState>
    {

        public TestProcessState Create(object message, TestProcessState state)
        {
            switch (message)
            {
                case TestProcessStartMessage e: return new TestProcessState(Guid.NewGuid(), nameof(TestProcess.Initial));
            }
            return state;
        }
    }
}