using GridDomain.Tests.CommandsExecution;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    public class Persisted_SyncExecute_until_projection_build_event_wait_by_caller: SyncExecute_until_projection_build_event_wait_by_caller
    {
        public Persisted_SyncExecute_until_projection_build_event_wait_by_caller():base(false)
        {
            
        }
      
    }
}