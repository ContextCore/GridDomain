using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    public class Persisted_Async_execute_dont_wait : InMemory_Async_execute_dont_wait
    {
        public Persisted_Async_execute_dont_wait():base(false)
        {
            
        }
    }
}