using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.CommandsExecution.ExpectedMessages;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute
{
    [TestFixture]
    public class Persisted_When_SyncExecute_until_aggregate_event_wait_by_caller : SyncExecute_until_aggregate_event_wait_by_caller
    {

        public Persisted_When_SyncExecute_until_aggregate_event_wait_by_caller():base(false)
        {
            
        }
     
    }
}