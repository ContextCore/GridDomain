using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
    public class Given_saga_When_publishing_start_message_A : SoftwareProgrammingInstanceSagaTest
    {
        [Fact]
        public async Task When_publishing_start_message()
        {
            GotTiredEvent startMessage = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            await GridNode.NewDebugWaiter()
                          .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                          .Create()
                          .SendToSagas(startMessage);

            var sagaData = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(startMessage.SagaId);
            //Saga_has_correct_data()
            Assert.Equal(startMessage.PersonId, sagaData.Data.PersonId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingSaga.MakingCoffee), sagaData.Data.CurrentStateName);
        }
    }
}