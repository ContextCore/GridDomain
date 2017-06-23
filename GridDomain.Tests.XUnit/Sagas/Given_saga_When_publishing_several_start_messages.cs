using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit.CommandsExecution;
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
        public async Task Then_separate_saga_startes_on_each_message()
        {
            var startMessageA = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var resA = await Node.NewDebugWaiter()
                                 .Expect<SagaReceivedMessage<SoftwareProgrammingState>>()
                                 .Create()
                                 .SendToSagas(startMessageA);

            var stateA = resA.Message<SagaReceivedMessage<SoftwareProgrammingState>>().State;

            //will reach same saga as already created and will produce a new one
            var secondStartMessageB = new SleptWellEvent(Guid.NewGuid(), Guid.NewGuid(), stateA.Id);

            var resB = await Node.NewDebugWaiter()
                                 .Expect<SagaReceivedMessage<SoftwareProgrammingState>>()
                                 .Create()
                                 .SendToSagas(secondStartMessageB);

            var stateB = resB.Message<SagaReceivedMessage<SoftwareProgrammingState>>().State;

            Assert.Equal(secondStartMessageB.SofaId, stateB.SofaId);
            //Saga_has_correct_state()
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), stateB.CurrentStateName);
            Assert.NotEqual(stateA.Id, stateB.Id);
        }
    }
}