using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;

using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Tests.Common;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;
using Xunit;
using Xunit.Abstractions;
using ActorSystem = Akka.Actor.ActorSystem;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_SagaActor_When_valid_Transitions
    { 
        public Given_SagaActor_When_valid_Transitions(ITestOutputHelper output)
        {
            _log = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        private readonly ILogger _log;

        [Fact]
        public async Task Commands_are_produced()
        {
            var given = new Given_AutomatonymousSaga(m => m.Coding, _log);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            var newState = await ((ISaga<SoftwareProgrammingState>) given.SagaInstance).PreviewTransit(subscriptionExpiredEvent);

            Assert.NotEmpty(newState.ProducedCommands);
        }

        [Fact]
        public async Task SagaData_is_changed_after_transition_by_event_data()
        {
            var given = new Given_AutomatonymousSaga(m => m.Coding, _log);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            var newState = await ((ISaga<SoftwareProgrammingState>) given.SagaInstance).PreviewTransit(subscriptionExpiredEvent);

            Assert.Equal(subscriptionExpiredEvent.SourceId, newState.State.PersonId);
        }

        [Fact]
        public async Task State_in_transition_result_is_changed()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, _log);
            var newState = await ((ISaga<SoftwareProgrammingState>) given.SagaInstance).PreviewTransit(new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), newState.State.CurrentStateName);
        }

        [Fact]
        public async Task Saga_state_not_changed()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, _log);

            var stateBefore = given.SagaInstance.State.CurrentStateName;

            await given.SagaInstance.PreviewTransit(new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));

            var stateAfter = given.SagaInstance.State.CurrentStateName;

            Assert.Equal(stateBefore, stateAfter);
        }

        [Fact]
        public async Task State_is_changed_on_using_non_generic_transit_method()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, _log);
            object msg = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());
            var newState =  await given.SagaInstance.PreviewTransit((dynamic)msg);
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), newState.State.CurrentStateName);
        }

        [Fact]
        public async Task When_apply_known_but_not_mapped_event_in_state()
        {
            var given = new Given_AutomatonymousSaga(m => m.Sleeping, _log);
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            await XUnitAssertExtensions.ShouldThrow<SagaTransitionException>(given.SagaInstance.PreviewTransit(gotTiredEvent));
        }
    }
}