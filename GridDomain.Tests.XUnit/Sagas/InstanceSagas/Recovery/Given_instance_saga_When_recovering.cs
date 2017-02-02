using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;
using Xunit;
using Assert = Xunit.Assert;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas.Recovery
{
    public class Given_instance_saga_When_recovering
    {
        [Fact]
        public async Task Given_instance_saga_When_recovering_from_creation()
        {
            var aggregateFactory = new AggregateFactory();
            var sagaId = Guid.NewGuid();

            var data = aggregateFactory.Build<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId);
            var saga = new SoftwareProgrammingSaga();
            var initialState = new SoftwareProgrammingSagaData(sagaId, saga.MakingCoffee.Name);

            var eventsToReplay = new DomainEvent[] {new SagaCreatedEvent<SoftwareProgrammingSagaData>(initialState, sagaId)};

            data.ApplyEvents(eventsToReplay);

            var sagaInstance = SagaInstance.New(saga, data);

            //Try to transit saga by message, available only in desired state
            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(), Guid.NewGuid());
            await sagaInstance.Transit(coffeMakeFailedEvent);
            var dispatchedCommands = sagaInstance.CommandsToDispatch;
            //Saga_produce_commands_only_one_command()
            Assert.Equal(1, dispatchedCommands.Count);
            //Produced_command_has_right_person_id()
            var sleepCommand = dispatchedCommands.OfType<GoSleepCommand>()
                                                 .First();
            Assert.Equal(coffeMakeFailedEvent.ForPersonId, sleepCommand.PersonId);
            //Produced_command_has_right_sofa_id()
            Assert.Equal(data.Data.SofaId, sleepCommand.SofaId);
            //Saga_produce_command_from_given_state()
            Assert.IsAssignableFrom<GoSleepCommand>(dispatchedCommands.FirstOrDefault());
        }
    }
}