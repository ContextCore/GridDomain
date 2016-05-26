using System;
using GridDomain.Balance.Domain;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Balance
{
    [TestFixture]
    public class BuySubscriptionSagaTests
    {
        [Test]
        public void GetSagaGraph()
        {
            var saga = new BuySubscriptionSaga();
            var graph = saga.Machine.ToDotGraph();
            Console.WriteLine(graph);
        }
    }
}
