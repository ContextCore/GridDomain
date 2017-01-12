using GridDomain.Tests.Unit.CommandsExecution;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.SynchroniousCommandExecute.ExpectedMessages
{
    [TestFixture]
    public class PersistedWhenWhenExecuteCommandThenAggregateEventWaitByCaller : When_execute_command_Then_aggregate_Should_persist_changed
    {

        public PersistedWhenWhenExecuteCommandThenAggregateEventWaitByCaller():base(false)
        {
            
        }
     
    }
}