using System;
using System.Linq;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
   
    public class CreateSagaGraph
    {
        private readonly ITestOutputHelper _helper;

        public CreateSagaGraph(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        [Fact]
        public void GetGraph()
        {
            var saga = new SoftwareProgrammingSaga();
            var generator = new StateMachineGraphvizGenerator(saga.GetGraph());
            var dotFile = generator.CreateDotFile();
            _helper.WriteLine(dotFile);
            Assert.True(dotFile.Any());
        }
    }
}