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
    public class ProcessManagerProducedEventsAndCommandsHasProcessId : NodeTestKit
    {
        protected ProcessManagerProducedEventsAndCommandsHasProcessId(NodeTestFixture fixture) : base(fixture) { }
        public ProcessManagerProducedEventsAndCommandsHasProcessId(ITestOutputHelper helper) : this(new SoftwareProgrammingProcessManagerFixture(helper)) { }

        [Fact]
        public async Task When_process_created_from_event_with_processId_new_Id_is_generated()
        {
            var domainEvent = new GotTiredEvent(Guid.NewGuid()
                                                    .ToString(),
                                                Guid.NewGuid()
                                                    .ToString(),
                                                Guid.NewGuid()
                                                    .ToString());

            var waitResults = await Node.PrepareForProcessManager(domainEvent)
                                        .Expect<ProcessManagerCreated<SoftwareProgrammingState>>()
                                        .Send();

            Assert.NotEqual(domainEvent.ProcessId,
                            waitResults.Message<ProcessManagerCreated<SoftwareProgrammingState>>()
                                       .State.Id);
        }

        [Fact]
        public async Task When_dispatch_command_than_command_should_have_right_processId()
        {
            var messages = await Node.PrepareForProcessManager(new GotTiredEvent(Guid.NewGuid()
                                                                                     .ToString()))
                                     .Expect<MakeCoffeCommand>()
                                     .And<ProcessManagerCreated<SoftwareProgrammingState>>()
                                     .Send();

            var command = messages.Message<MakeCoffeCommand>();
            var processCreatedEvent = messages.Message<ProcessStateEvent>();

            Assert.Equal(processCreatedEvent.SourceId,
                         command.ProcessId);

            Assert.IsAssignableFrom<MakeCoffeCommand>(command);
        }
    }
}