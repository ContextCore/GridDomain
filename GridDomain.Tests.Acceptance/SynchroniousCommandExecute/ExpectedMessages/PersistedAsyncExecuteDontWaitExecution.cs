using GridDomain.Tests.Unit.CommandsExecution;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute.ExpectedMessages
{
    [TestFixture]
    public class PersistedAsyncExecuteExecuteCommandWithoutExpectations : When_execute_command_without_expectations
    {
        public PersistedAsyncExecuteExecuteCommandWithoutExpectations():base(false)
        {
            
        }
    }
}