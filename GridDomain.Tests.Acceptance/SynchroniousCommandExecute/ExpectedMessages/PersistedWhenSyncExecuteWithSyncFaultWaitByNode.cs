using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.CommandsExecution.ExpectedMessages;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    public class PersistedWhenSyncExecuteWithSyncFaultWaitByNode : SyncExecute_with_sync_fault_wait_by_Node
    {
        public PersistedWhenSyncExecuteWithSyncFaultWaitByNode():base(false)
        {
            
        }
    }
}