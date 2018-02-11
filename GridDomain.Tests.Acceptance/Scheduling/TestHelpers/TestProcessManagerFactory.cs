using System;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class TestProcessManagerFactory : IProcessStateFactory<TestProcessState>
    {

        public TestProcessState Create(object message)
        {
            switch (message)
            {
                case TestProcessStartMessage e: return new TestProcessState(Guid.NewGuid().ToString(), nameof(TestProcess.Initial));
            }
            throw new CannotCreateStateFromMessageException(message);
        }
    }
}