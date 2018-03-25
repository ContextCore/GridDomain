using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class ProcessManagerProducedEventsAndCommandsHasProcessId : SoftwareProgrammingProcessTest
    {
        public ProcessManagerProducedEventsAndCommandsHasProcessId(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task When_dispatch_command_than_command_should_have_right_processId()
        {
            Node.Pipe.ProcessesPipeActor.Tell(new Initialize(TestActor));

            var processCreatedMsg = await Node.NewLocalDebugWaiter()
                                              .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                              .Create()
                                              .SendToProcessManagers(new GotTiredEvent(Guid.NewGuid().ToString()));

            var processCompleteMsg = FishForMessage<IMessageMetadataEnvelop<ICommand>>(m => true, TimeSpan.FromHours(1));
            var command = processCompleteMsg.Message;

            Assert.Equal(processCreatedMsg.Message<ProcessStateEvent>()
                                          .SourceId,
                         command.ProcessId);
            Assert.IsAssignableFrom<MakeCoffeCommand>(command);
        }

        [Fact]
        public async Task When_process_created_from_event_with_processId_new_Id_is_generated()
        {
            var domainEvent = new GotTiredEvent(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var waitResults = await Node.NewLocalDebugWaiter()
                                        .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                        .Create()
                                        .SendToProcessManagers(domainEvent);

            Assert.NotEqual(domainEvent.ProcessId,
                            waitResults.Message<ProcessManagerCreated<SoftwareProgrammingState>>()
                                       .State.Id);
        }
    }
}