using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenew;
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
                new SubscriptionRenewSaga(new SubscriptionRenewSagaState(Guid.NewGuid(),SubscriptionRenewSaga.States.SubscriptionSet));
            Console.WriteLine(saga.Machine.ToDotGraph());
        }
    }
}