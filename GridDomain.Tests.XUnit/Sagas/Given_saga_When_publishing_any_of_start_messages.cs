using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging.Waiting;
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
            var res = await
                Node.NewDebugWaiter()
                    .Expect<SagaCreated<SoftwareProgrammingState>>()
                    .Create()
                    .SendToSagas(new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid()));

            var sagaCreatedEvent = res.Message<SagaCreated<SoftwareProgrammingState>>();

            var state = await this.LoadSaga<SoftwareProgrammingState>(sagaCreatedEvent.State.Id);
            //Saga_data_is_not_null()
            Assert.NotNull(state);
            // Saga_has_correct_id()
            Assert.Equal(sagaCreatedEvent.SourceId, state.Id);
        }
    }
}