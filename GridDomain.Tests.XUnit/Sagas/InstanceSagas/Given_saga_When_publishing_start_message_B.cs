using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
    public class Given_saga_When_publishing_start_message_B : SoftwareProgrammingInstanceSagaTest
    {
        [Fact]
        public void When_publishing_start_message()
        {
            SleptWellEvent startMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            GridNode.NewDebugWaiter()
                    .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                    .Create()
                    .SendToSagas(startMessage)
                    .Wait();

            var sagaData = LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(startMessage.SagaId);
            // Saga_has_correct_data()
            Assert.Equal(startMessage.SofaId, sagaData.Data.SofaId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Coding), sagaData.Data.CurrentStateName);
        }
    }
}