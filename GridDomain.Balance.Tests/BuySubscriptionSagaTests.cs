using System;
using GridDomain.Balance.Domain;
using GridDomain.EventSourcing.Sagas;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Balance
{
    [TestFixture]
    public class BuySubscriptionSagaTests
    {
        [Test]
        public void GetSagaGraph()
        {
            var sagaStateAggregate = new SagaStateAggregate<BuySubscriptionSaga.State,
                                                            BuySubscriptionSaga.Transitions>
                                       (Guid.NewGuid(), BuySubscriptionSaga.State.SubscriptionExist);

            var saga = new BuySubscriptionSaga(sagaStateAggregate);
            var graph = saga.Machine.ToDotGraph();
            Console.WriteLine(graph);
        }
    }
}