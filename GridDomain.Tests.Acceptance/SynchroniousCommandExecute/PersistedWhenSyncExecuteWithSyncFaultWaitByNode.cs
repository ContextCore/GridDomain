using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class PersistedWhenSyncExecuteWithSyncFaultWaitByNode : InMemory_When_SyncExecute_with_sync_fault_wait_by_Node
    {
        public PersistedWhenSyncExecuteWithSyncFaultWaitByNode():base(false)
        {
            
        }
    }
}