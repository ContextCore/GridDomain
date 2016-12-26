using GridDomain.Tests.Unit.CommandsExecution.ExpectedMessages;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute.ExpectedMessages
{
    [TestFixture]
    public class Persisted_SyncExecute_until_projection_build_event_wait_by_caller: SyncExecute_until_projection_build_event_wait_by_caller
    {
        public Persisted_SyncExecute_until_projection_build_event_wait_by_caller():base(false)
        {
            
        }
    }
}