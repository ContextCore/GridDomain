using GridDomain.Tests.Unit.CommandsExecution.ExpectedMessages;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute.ExpectedMessages
{
    [TestFixture]
    public class PersistedWhenSyncExecuteWithSyncFaultWaitByNode : SyncExecute_with_sync_fault_wait_by_Node
    {
        public PersistedWhenSyncExecuteWithSyncFaultWaitByNode():base(false)
        {
            
        }
    }
}