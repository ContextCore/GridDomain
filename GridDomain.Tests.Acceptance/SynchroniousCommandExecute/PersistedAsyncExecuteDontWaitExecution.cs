using GridDomain.Tests.CommandsExecution;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    public class PersistedAsyncExecuteDontWaitExecution : When_dont_wait_execution
    {
        public PersistedAsyncExecuteDontWaitExecution():base(false)
        {
            
        }
    }
}