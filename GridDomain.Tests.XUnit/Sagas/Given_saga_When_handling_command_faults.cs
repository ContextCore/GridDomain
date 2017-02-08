using System;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_saga_When_handling_command_faults : NodeTestKit
    {
        public Given_saga_When_handling_command_faults(ITestOutputHelper output) : base(output, new FaultyAggregateFixture()) {}

        class FaultyAggregateFixture : SoftwareProgrammingSagaFixture
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
            var sagaId = Guid.NewGuid();

            var sagaData = new SoftwareProgrammingSagaData(sagaId, nameof(SoftwareProgrammingSaga.MakingCoffee))
                           {
                               PersonId = Guid.NewGuid()
                           };

            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSagaData>(sagaData, sagaId);

            await Sys.SaveToJournal<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId, sagaDataEvent);

            await Task.Delay(100);
            var coffeMakeFailedEvent = new CoffeMakeFailedEvent(Guid.NewGuid(),
                                                                Guid.NewGuid(),
                                                                BusinessDateTime.UtcNow,
                                                                sagaId);

            await Node.NewDebugWaiter(TimeSpan.FromMinutes(10))
                      .Expect<SagaMessageReceivedEvent<SoftwareProgrammingSagaData>>(
                          m => m.SagaData.CurrentStateName == nameof(SoftwareProgrammingSaga.Coding))
                      .Create()
                      .SendToSagas(coffeMakeFailedEvent, new MessageMetadata(coffeMakeFailedEvent.SourceId));

            await Task.Delay(1000);
            var sagaDataAggregate = await this.LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(sagaId);
            //Saga_should_be_in_correct_state_after_fault_handling()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Coding), sagaDataAggregate.Data.CurrentStateName);
            //Saga_state_should_contain_data_from_fault_message()
            Assert.Equal(coffeMakeFailedEvent.ForPersonId, sagaData.BadSleepPersonId);
        }
    }
}