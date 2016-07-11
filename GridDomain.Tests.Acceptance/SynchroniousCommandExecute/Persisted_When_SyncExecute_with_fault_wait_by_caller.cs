using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class Persisted_When_SyncExecute_with_fault_wait_by_caller : InMemory_When_SyncExecute_with_fault_wait_by_caller
    {
        public Persisted_When_SyncExecute_with_fault_wait_by_caller():base(false)
        {
            
        }
    }
}