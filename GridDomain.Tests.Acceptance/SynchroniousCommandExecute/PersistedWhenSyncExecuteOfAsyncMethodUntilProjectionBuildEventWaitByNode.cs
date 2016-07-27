using GridDomain.Tests.AsyncAggregates;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    public class PersistedWhenSyncExecuteOfAsyncMethodUntilProjectionBuildEventWaitByNode : When_wait_execution_of_async_method_until_projection_build_event_wait_by_Node
    {
        public PersistedWhenSyncExecuteOfAsyncMethodUntilProjectionBuildEventWaitByNode() : base(false)
        {


        }
   
    }
}