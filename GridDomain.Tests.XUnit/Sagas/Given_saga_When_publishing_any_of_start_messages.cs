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
    public class Given_saga_When_publishing_any_of_start_messages : SoftwareProgrammingSagaTest
    {
        public Given_saga_When_publishing_any_of_start_messages(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_publishing_start_message()
        {
            var sagaId = Guid.NewGuid();
            await
                Node.NewDebugWaiter()
                    .Expect<SagaCreatedEvent<SoftwareProgrammingSagaState>>()
                    .Create()
                    .SendToSagas(new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), sagaId));

            var sagaData = await this.LoadAggregate<SagaStateAggregate<SoftwareProgrammingSagaState>>(sagaId);
            //Saga_data_is_not_null()
            Assert.NotNull(sagaData.Data);
            // Saga_has_correct_id()
            Assert.Equal(sagaId, sagaData.Id);
        }
    }
}