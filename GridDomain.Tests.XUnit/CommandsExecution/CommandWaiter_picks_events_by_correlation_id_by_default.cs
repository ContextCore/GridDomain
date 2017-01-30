using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class CommandWaiter_picks_events_by_correlation_id_by_default : SampleDomainCommandExecutionTests
    {
        public CommandWaiter_picks_events_by_correlation_id_by_default(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Default_wait_for_event_matches_by_correlation_id_when_projection_builder_cares_about_metadata()
        {
            var commandA = new LongOperationCommand(10, Guid.NewGuid());
            var commandB = new LongOperationCommand(10, Guid.NewGuid());

            Node.Execute(commandB);
            var res =  await Node.Prepare(commandA)
                                     .Expect<AggregateChangedEventNotification>()
                                     .Execute(false);
                              
            //will pick right command by correlation Id
            Assert.Equal(commandA.AggregateId, res.Message<AggregateChangedEventNotification>().AggregateId);
        }
    }
}
