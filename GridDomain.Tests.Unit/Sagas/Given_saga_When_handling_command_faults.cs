using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_saga_When_handling_command_faults : NodeTestKit
    {
        public Given_saga_When_handling_command_faults(ITestOutputHelper output) : base(output, new FaultyAggregateFixture()) {}

        private class FaultyAggregateFixture : SoftwareProgrammingSagaFixture
        {
            public FaultyAggregateFixture()
            {
                Add(new CustomContainerConfiguration(c => c.RegisterAggregate<HomeAggregate, HomeAggregateHandler>()));
                Add(new CustomRouteMap(r => r.RegisterAggregate(HomeAggregateHandler.Descriptor)));
            }
        }

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

            var sagaDataAggregate = await this.LoadAggregate<SagaStateAggregate<SoftwareProgrammingState>>(givenSagaStateAggregate.Id);
            //Saga_should_be_in_correct_state_after_fault_handling()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), sagaDataAggregate.State.CurrentStateName);
            //Saga_state_should_contain_data_from_fault_message()
            Assert.Equal(coffeMakeFailedEvent.ForPersonId, sagaDataAggregate.State.BadSleepPersonId);
        }
    }
}