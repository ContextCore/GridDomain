using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    public class Persisted_SyncExecute_until_projection_build_event_wait_by_caller: InMemory_SyncExecute_until_projection_build_event_wait_by_caller
    {
        public Persisted_SyncExecute_until_projection_build_event_wait_by_caller():base(false)
        {
            
        }
      
    }
}