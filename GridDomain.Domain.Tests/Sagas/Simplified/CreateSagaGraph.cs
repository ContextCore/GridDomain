using System;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga;
using NUnit.Framework;
using Automatonymous.Graphing;

namespace GridDomain.Tests.Sagas.Simplified
{
    [TestFixture]
    internal class CreateSagaGraph
    {
        [Test]
        public void GetGraph()
        {
            var saga = new SubscriptionRenewSagaSimplified();
            var generator = new StateMachineGraphvizGenerator(saga.GetGraph());
            Console.WriteLine(generator.CreateDotFile());
        }
    }
}