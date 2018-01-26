using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers.Recovery
{
    public class Given_instance_process_When_recovering
    {
        public Given_instance_process_When_recovering(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        private readonly Logger _logger;

        [Fact]
        public async Task Given_instance_process_When_recovering_from_creation()
        {
            var aggregateFactory = new AggregateFactory();
            var processId = Guid.NewGuid();

            var data = aggregateFactory.Build<ProcessStateAggregate<SoftwareProgrammingState>>(processId);
            var process = new SoftwareProgrammingProcess();
            var initialState = new SoftwareProgrammingState(processId, process.MakingCoffee.Name);

            var eventsToReplay = new DomainEvent[] {new ProcessManagerCreated<SoftwareProgrammingState>(initialState, processId)};

            data.ApplyEvents(eventsToReplay);

            var processManager = new SoftwareProgrammingProcess();

            //Try to transit process by message, available only in desired state
            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(), Guid.NewGuid());
            var dispatchedCommands = await processManager.Transit(data.State,coffeMakeFailedEvent);
            //process_produce_commands_only_one_command()
            Assert.Equal(1, dispatchedCommands.Count);
            //Produced_command_has_right_person_id()
            var sleepCommand = dispatchedCommands.OfType<GoSleepCommand>().First();
            Assert.Equal(coffeMakeFailedEvent.ForPersonId, sleepCommand.PersonId);
            //Produced_command_has_right_sofa_id()
            Assert.Equal(data.State.SofaId, sleepCommand.SofaId);
            //process_produce_command_from_given_state()
            Assert.IsAssignableFrom<GoSleepCommand>(dispatchedCommands.FirstOrDefault());
        }
    }
}