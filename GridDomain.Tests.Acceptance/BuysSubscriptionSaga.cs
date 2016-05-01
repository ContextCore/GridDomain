using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Balance.Domain;
using NUnit.Framework;

namespace GridDomain.Domain.Tests
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
