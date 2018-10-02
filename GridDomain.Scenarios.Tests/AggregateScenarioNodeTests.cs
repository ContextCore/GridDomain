using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.Scenarios.Runners;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Xunit.Abstractions;

namespace GridDomain.Scenarios.Tests {
    public class AggregateScenarioNodeTests : AggregateScenarioTests
    {
        protected override Task<IAggregateScenarioRun<T>> Run<T>(IAggregateScenario<T> scenario)
        {
            return scenario.Run()
                           .Node(new DomainConfiguration(new BalloonDomainConfiguration(),
                                                         new ProgrammerAggregateDomainConfiguration()),Logger);
        }

        public AggregateScenarioNodeTests(ITestOutputHelper output) : base(output) { }
    }
}