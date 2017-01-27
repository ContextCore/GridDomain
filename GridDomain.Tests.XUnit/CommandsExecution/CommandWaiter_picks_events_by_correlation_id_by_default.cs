using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    class CommandWaiter_picks_events_by_correlation_id_by_default : SampleDomainCommandExecutionTests
    {
        public CommandWaiter_picks_events_by_correlation_id_by_default() : base(true)
        {
        }

       [Fact]
        public async Task Default_wait_for_event_matches_by_correlation_id_when_projection_builder_cares_about_metadata()
        {
            var commandA = new LongOperationCommand(10, Guid.NewGuid());
            var commandB = new LongOperationCommand(10, Guid.NewGuid());

            GridNode.Execute(commandB);
            var res =  await Node.Prepare(commandA)
                                     .Expect<AggregateChangedEventNotification>()
                                     .Execute(false);
                              
            //will pick right command by correlation Id
            Assert.Equal(commandA.AggregateId, res.Message<AggregateChangedEventNotification>().AggregateId);
        }
    }
}
