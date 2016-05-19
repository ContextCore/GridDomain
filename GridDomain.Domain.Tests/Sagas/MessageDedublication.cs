using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting.Sagas;
using GridDomain.Domain.Tests;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Sagas
{
    
    [TestFixture]
    class Given_several_repeating_messages_should_dispatch_only_one_command
    {
        private SubscriptionRenewSaga Saga;
        private NotEnoughFondsFailure[] Messages;
        public void Given_new_saga_with_state()
        {
            var sagaState = new SagaStateAggregate<SubscriptionRenewSaga.States,
                                 SubscriptionRenewSaga.Triggers>(Guid.NewGuid(), SubscriptionRenewSaga.States.OfferPaying);

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
            foreach(var m in Messages)
                Saga.Handle(m);
        }

        [SetUp]
        public void Init()
        {
            Given_new_saga_with_state();
            When_applying_several_events();
        }

        [Then]
        public void Only_one_command_should_be_dispatched()
        {
            Assert.AreEqual(1, Saga.MessagesToDispatch.Count);
        }

        [Then]
        public void All_dispatched_messages_are_commands()
        {
            CollectionAssert.AllItemsAreInstancesOfType(Saga.MessagesToDispatch, typeof(ChangeSubscriptionCommand));
        }
    }
}
