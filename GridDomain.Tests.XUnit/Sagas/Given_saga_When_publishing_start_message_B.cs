using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_saga_When_publishing_start_message_B : SoftwareProgrammingInstanceSagaTest
    {
        public Given_saga_When_publishing_start_message_B(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_publishing_start_message()
        {
            var startMessage = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            Node.NewDebugWaiter()
                .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                .Create()
                .SendToSagas(startMessage)
                .Wait();

            var sagaData = await this.LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>>(startMessage.SagaId);
            // Saga_has_correct_data()
            Assert.Equal(startMessage.SofaId, sagaData.Data.SofaId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingSaga.Coding), sagaData.Data.CurrentStateName);
        }
    }
}