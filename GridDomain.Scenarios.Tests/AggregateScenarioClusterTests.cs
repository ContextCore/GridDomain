using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.Scenarios.Runners;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Xunit.Abstractions;

namespace GridDomain.Scenarios.Tests {
    public class AggregateScenarioClusterTests : AggregateScenarioTests
    {
        private readonly ITestOutputHelper _output;

        protected override Task<IAggregateScenarioRun<T>> Run<T>(IAggregateScenario<T> scenario)
        {
            return scenario.Run()
                           .Cluster(new DomainConfiguration(new BalloonDomainConfiguration(),
                                                            new ProgrammerAggregateDomainConfiguration()),() =>new XUnitAutoTestLoggerConfiguration(_output));
        }

        public AggregateScenarioClusterTests(ITestOutputHelper output) : base(output)
        {
            _output = output;
        }
    }
}