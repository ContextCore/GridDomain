using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    public class Persisted_When_SyncExecute_until_projection_build_event_wait_by_Node : In_Memory_When_SyncExecute_until_projection_build_event_wait_by_Node
    {
        public Persisted_When_SyncExecute_until_projection_build_event_wait_by_Node() : base(false)
        {


        }
   
    }
}