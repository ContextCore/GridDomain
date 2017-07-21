using System.Linq;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class CreateProcessGraph
    {
        public CreateProcessGraph(ITestOutputHelper helper)
        {
            _helper = helper;
        }

        private readonly ITestOutputHelper _helper;

        [Fact]
        public void GetGraph()
        {
            var process = new SoftwareProgrammingProcess();
            var generator = new StateMachineGraphvizGenerator(process.GetGraph());
            var dotFile = generator.CreateDotFile();
            _helper.WriteLine(dotFile);
            Assert.True(dotFile.Any());
        }
    }
}