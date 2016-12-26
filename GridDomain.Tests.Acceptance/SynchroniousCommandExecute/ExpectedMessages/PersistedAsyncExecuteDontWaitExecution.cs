using GridDomain.Tests.Unit.CommandsExecution.ExpectedMessages;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute.ExpectedMessages
{
    [TestFixture]
    public class PersistedAsyncExecuteDontWaitExecution : When_dont_wait_execution
    {
        public PersistedAsyncExecuteDontWaitExecution():base(false)
        {
            
        }
    }
}