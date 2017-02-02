using System;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
   
    public class CreateSagaGraph
    {
        [Fact]
        public void GetGraph()
        {
            var saga = new SoftwareProgrammingSaga();
            var generator = new StateMachineGraphvizGenerator(saga.GetGraph());
            Console.WriteLine(generator.CreateDotFile());
        }
    }
}