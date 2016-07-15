using System;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    internal class CreateSagaGraph
    {
        [Test]
        public void GetGraph()
        {
            var saga = new SubscriptionRenewSaga();
            var generator = new StateMachineGraphvizGenerator(saga.GetGraph());
            Console.WriteLine(generator.CreateDotFile());
        }
    }
}