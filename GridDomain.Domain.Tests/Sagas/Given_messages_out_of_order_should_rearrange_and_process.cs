using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;
using States = GridDomain.Tests.Sagas.SubscriptionRenewSaga.SubscriptionRenewSaga.States;

namespace GridDomain.Tests.Sagas
{
    //TODO: create tests after finding good business case
    [TestFixture]
    class Given_messages_out_of_order_should_rearrange_and_process
    {
        //private SubscriptionRenewSaga.SubscriptionRenewSaga Saga;
        //private NotEnoughFondsFailure[] Messages;

        //public void Given_new_saga_with_state()
        //{
        //    var sagaState = new SagaStateAggregate<States,
        //        SubscriptionRenewSaga.SubscriptionRenewSaga.Triggers>(Guid.NewGuid(), States.SubscriptionSet);

        //    Saga = new SubscriptionRenewSaga.SubscriptionRenewSaga(sagaState);


        //    Messages = new[]
        //    {
        //        new Revoke(),
        //        new NotEnoughFondsFailure(),
        //        new NotEnoughFondsFailure(),
        //        new NotEnoughFondsFailure()
        //    };
        //}

        //public void When_applying_several_events()
        //{
        //    foreach (var m in Messages)
        //        Saga.Handle(m);
        //}



    }
}
