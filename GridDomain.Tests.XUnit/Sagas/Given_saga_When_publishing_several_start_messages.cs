using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class Given_saga_When_publishing_several_start_messages : SoftwareProgrammingSagaTest
    {
        public Given_saga_When_publishing_several_start_messages(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_publishing_start_message()
        {
            var sagaId = Guid.NewGuid();
            var startMessageA = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), sagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaCreatedEvent<SoftwareProgrammingState>>()
                      .Create()
                      .SendToSagas(startMessageA);

            var secondStartMessageB = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), sagaId);

            await Node.NewDebugWaiter()
                      .Expect<SagaCreatedEvent<SoftwareProgrammingState>>()
                      .Create()
                      .SendToSagas(secondStartMessageB);

            var sagaData = await this.LoadAggregate<SagaStateAggregate<SoftwareProgrammingState>>(sagaId);
            //Saga_reinitialized_from_last_start_message()
            Assert.Equal(secondStartMessageB.SofaId, sagaData.SagaState.SofaId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), sagaData.SagaState.CurrentStateName);
        }
    }
}