using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenew;
using GridDomain.Tests.Sagas.SubscriptionRenew.Commands;
using GridDomain.Tests.Sagas.SubscriptionRenew.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas
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

        private SubscriptionRenew.SubscriptionRenewSaga Saga;
        private NotEnoughFondsFailure[] Messages;

        public void Given_new_saga_with_state()
        {
            var sagaState = new SubscriptionRenewSagaState(Guid.NewGuid(),SubscriptionRenewSaga.States.OfferPaying);

            Saga = new SubscriptionRenewSaga(sagaState);


            Messages = new[]
            {
                new NotEnoughFondsFailure(),
                new NotEnoughFondsFailure(),
                new NotEnoughFondsFailure(),
                new NotEnoughFondsFailure()
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
            CollectionAssert.AllItemsAreInstancesOfType(Saga.CommandsToDispatch, typeof (ChangeSubscriptionCommand));
        }
    }
}