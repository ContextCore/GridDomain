using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class Persisted_When_SyncExecute_with_fault_wait_by_Node : InMemory_When_SyncExecute_with_fault_wait_by_Node
    {
        public Persisted_When_SyncExecute_with_fault_wait_by_Node():base(false)
        {
            
        }
    }
}