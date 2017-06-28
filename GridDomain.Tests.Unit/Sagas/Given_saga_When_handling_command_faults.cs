using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Integration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Sagas
{

    public class Given_saga_When_handling_command_faults : NodeTestKit
    {
        public Given_saga_When_handling_command_faults(ITestOutputHelper output) :
            base(output, new SoftwareProgrammingSagaFixture(new HomeDomainConfiguration())) {}

        [Fact]
        public async Task When_saga_produce_command_and_waiting_for_it_fault()
        {

            var givenSagaStateAggregate = new SagaStateAggregate<SoftwareProgrammingState>(new SoftwareProgrammingState(Guid.NewGuid(), 
                                                                                            nameof(SoftwareProgrammingProcess.MakingCoffee))
                                                                                          {
                                                                                              PersonId = Guid.NewGuid()
                                                                                          });

            await Node.SaveToJournal(givenSagaStateAggregate);

            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(),
                                                                givenSagaStateAggregate.State.PersonId,
                                                                BusinessDateTime.UtcNow,
                                                                givenSagaStateAggregate.Id);

            await Node.NewDebugWaiter()
                      .Expect<SagaReceivedMessage<SoftwareProgrammingState>>(m => m.State.CurrentStateName == nameof(SoftwareProgrammingProcess.Coding))
                      .Create()
                      .SendToSagas(coffeMakeFailedEvent, new MessageMetadata(coffeMakeFailedEvent.SourceId));

            var sagaDataAggregate = await this.LoadSagaByActor<SoftwareProgrammingState>(givenSagaStateAggregate.Id);
            //Saga_should_be_in_correct_state_after_fault_handling()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), sagaDataAggregate.CurrentStateName);
            //Saga_state_should_contain_data_from_fault_message()
            Assert.Equal(coffeMakeFailedEvent.ForPersonId, sagaDataAggregate.BadSleepPersonId);
        }
    }
}