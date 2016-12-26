using System;
using System.Threading.Tasks;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.MessageWaiting.Commanding
{
    [TestFixture]
    class CommandWaiter_picks_events_by_correlation_id_by_default : SampleDomainCommandExecutionTests
    {
        public CommandWaiter_picks_events_by_correlation_id_by_default() : base(true)
        {
        }

        [Test]
        public async Task Default_wait_for_event_matches_by_correlation_id_when_projection_builder_cares_about_metadata()
        {
            var commandA = new LongOperationCommand(10, Guid.NewGuid());
            var commandB = new LongOperationCommand(10, Guid.NewGuid());

            GridNode.Execute(commandB);
            var res =  await GridNode.PrepareCommand(commandA)
                                     .Expect<AggregateChangedEventNotification>()
                                     .Execute(TimeSpan.FromSeconds(500), false);
                              
            //will pick right command by correlation Id
            Assert.AreEqual(commandA.AggregateId, res.Message<AggregateChangedEventNotification>().AggregateId);
        }

    }
}
