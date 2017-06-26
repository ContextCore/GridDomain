using System.Linq;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Sagas
{
    public class CreateSagaGraph
    {
        public CreateSagaGraph(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        private readonly ITestOutputHelper _helper;

        [Fact]
        public void GetGraph()
        {
            var saga = new SoftwareProgrammingProcess();
            var generator = new StateMachineGraphvizGenerator(saga.GetGraph());
            var dotFile = generator.CreateDotFile();
            _helper.WriteLine(dotFile);
            Assert.True(dotFile.Any());
        }
    }
}