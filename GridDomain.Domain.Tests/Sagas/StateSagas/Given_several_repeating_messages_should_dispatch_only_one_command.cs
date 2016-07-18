using System;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Commands;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga.Events;
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
        private GotMoreTiredEvent[] Messages;

        public void Given_new_saga_with_state()
        {
            var sagaState = new SoftwareProgrammingSagaState(Guid.NewGuid(),SoftwareProgrammingSaga.States.DrinkingCoffe);

            Saga = new SoftwareProgrammingSaga(sagaState);


            Messages = new[]
            {
                new GotMoreTiredEvent(),
                new GotMoreTiredEvent(),
                new GotMoreTiredEvent(),
                new GotMoreTiredEvent()
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
            CollectionAssert.AllItemsAreInstancesOfType(Saga.CommandsToDispatch, typeof (SleepWellCommand));
        }
    }
}