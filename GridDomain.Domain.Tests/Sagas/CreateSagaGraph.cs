using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas
{
    [TestFixture]
    internal class CreateSagaGraph
    {
        [Test]
        public void GEtGraph()
        {
            var saga =
                new SubscriptionRenewSaga.SubscriptionRenewSaga(new SubscriptionRenewSagaState(Guid.NewGuid(),SubscriptionRenewSaga.SubscriptionRenewSaga.States.SubscriptionSet));
            Console.WriteLine(saga.Machine.ToDotGraph());
        }
    }
}