﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Processes;
using GridDomain.Processes.State;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Sagas.Recovery
{
    public class Given_instance_saga_When_recovering
    {
        public Given_instance_saga_When_recovering(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        private readonly Logger _logger;

        [Fact]
        public async Task Given_instance_saga_When_recovering_from_creation()
        {
            var aggregateFactory = new AggregateFactory();
            var sagaId = Guid.NewGuid();

            var data = aggregateFactory.Build<ProcessStateAggregate<SoftwareProgrammingState>>(sagaId);
            var saga = new SoftwareProgrammingProcess();
            var initialState = new SoftwareProgrammingState(sagaId, saga.MakingCoffee.Name);

            var eventsToReplay = new DomainEvent[] {new SagaCreated<SoftwareProgrammingState>(initialState, sagaId)};

            data.ApplyEvents(eventsToReplay);

            var sagaInstance = new ProcessManager<SoftwareProgrammingState>(saga,data.State, _logger);

            //Try to transit saga by message, available only in desired state
            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(), Guid.NewGuid());
            var newState = await sagaInstance.Transit(coffeMakeFailedEvent);
            var dispatchedCommands = newState.ProducedCommands;
            //Saga_produce_commands_only_one_command()
            Assert.Equal(1, dispatchedCommands.Count);
            //Produced_command_has_right_person_id()
            var sleepCommand = dispatchedCommands.OfType<GoSleepCommand>().First();
            Assert.Equal(coffeMakeFailedEvent.ForPersonId, sleepCommand.PersonId);
            //Produced_command_has_right_sofa_id()
            Assert.Equal(data.State.SofaId, sleepCommand.SofaId);
            //Saga_produce_command_from_given_state()
            Assert.IsAssignableFrom<GoSleepCommand>(dispatchedCommands.FirstOrDefault());
        }
    }
}