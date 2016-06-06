using System;
using GridDomain.EventSourcing.Sagas;
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
                new SubscriptionRenewSaga.SubscriptionRenewSaga(new SagaStateAggregate<SubscriptionRenewSaga.SubscriptionRenewSaga.States, SubscriptionRenewSaga.SubscriptionRenewSaga.Triggers>(Guid.NewGuid(), SubscriptionRenewSaga.SubscriptionRenewSaga.States.SubscriptionSet));
            Console.WriteLine(saga.Machine.ToDotGraph());
        }
    }
}