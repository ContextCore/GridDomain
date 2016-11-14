using System;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    [TestFixture]
    internal class Given_several_repeating_messages_should_dispatch_only_one_command
    {
        [SetUp]
        public void Init()
        {
            Given_new_saga_with_state();
            When_applying_several_events();
        }

        private SoftwareProgrammingSaga Saga;
        private CoffeMakeFailedEvent[] Messages;

        public void Given_new_saga_with_state()
        {
            var sagaState = new SoftwareProgrammingSagaState(Guid.NewGuid(),SoftwareProgrammingSaga.States.MakingCoffee);

            Saga = new SoftwareProgrammingSaga(sagaState);


            Messages = new[]
            {
                new CoffeMakeFailedEvent(sagaState.Id,sagaState.CoffeMachineId),
                new CoffeMakeFailedEvent(sagaState.Id,sagaState.CoffeMachineId),
                new CoffeMakeFailedEvent(sagaState.Id,sagaState.CoffeMachineId),
                new CoffeMakeFailedEvent(sagaState.Id,sagaState.CoffeMachineId)
            };
        }

        public void When_applying_several_events()
        {
            foreach (var m in Messages)
                Saga.Handle(m);
        }

        [Then]
        public void Only_one_command_should_be_dispatched()
        {
            Assert.AreEqual(1, Saga.CommandsToDispatch.Count);
        }

        [Then]
        public void All_dispatched_messages_are_commands()
        {
            CollectionAssert.AllItemsAreInstancesOfType(Saga.CommandsToDispatch, typeof (GoToWorkCommand));
        }
    }
}