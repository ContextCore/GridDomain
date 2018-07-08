using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class MetadataFromProcessReceivedEventPassedToProducedCommands : NodeTestKit
    {
        public MetadataFromProcessReceivedEventPassedToProducedCommands(ITestOutputHelper helper) : this(new SoftwareProgrammingProcessManagerFixture(helper)) { }
        protected MetadataFromProcessReceivedEventPassedToProducedCommands(NodeTestFixture fixture) : base(fixture) { }

        [Fact]
        public async Task When_publishing_start_message()
        {
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid()
                                                      .ToString(),
                                                  Guid.NewGuid()
                                                      .ToString(),
                                                  Guid.NewGuid()
                                                      .ToString());
            
            var gotTiredEventMetadata = MessageMetadata.New(gotTiredEvent.SourceId,
                                                            Guid.NewGuid()
                                                                .ToString(),
                                                            Guid.NewGuid()
                                                                .ToString());

            var answer = await Node.PrepareForProcessManager(gotTiredEvent, gotTiredEventMetadata)
                                   .Expect<MakeCoffeCommand>()
                                   .Send(TimeSpan.FromSeconds(5));

            var command = answer.Received;
            //Result_contains_metadata()
            var commandMetadata = answer.ReceivedMetadata;
            
            Assert.NotNull(commandMetadata);
            //Result_contains_message()
            Assert.NotNull(command);
            //Result_message_has_expected_value()
            Assert.Equal(gotTiredEvent.PersonId, command.PersonId);
            //Result_metadata_has_command_id_as_casuation_id()
            Assert.Equal(gotTiredEvent.SourceId, commandMetadata.CasuationId);
            //Result_metadata_has_correlation_id_same_as_command_metadata()
            Assert.Equal(gotTiredEventMetadata.CorrelationId, commandMetadata.CorrelationId);
        }
    }
}