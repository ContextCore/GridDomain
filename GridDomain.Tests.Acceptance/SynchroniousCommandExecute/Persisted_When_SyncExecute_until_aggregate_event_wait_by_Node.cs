using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    class Persisted_When_SyncExecute_until_aggregate_event_wait_by_Node : InMemory_When_SyncExecute_until_aggregate_event_wait_by_Node
    {

        public Persisted_When_SyncExecute_until_aggregate_event_wait_by_Node() : base(false)
        {


        }
    }
}