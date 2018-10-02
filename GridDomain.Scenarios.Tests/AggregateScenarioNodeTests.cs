using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.Scenarios.Runners;
using GridDomain.Tests.Acceptance;
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
    
    public class AggregateScenarioNodePersistenceTests : AggregateScenarioTests
    {
        protected override async Task<IAggregateScenarioRun<T>> Run<T>(IAggregateScenario<T> scenario)
        {
            var connectionString = "Server=localhost,1400; Database = AutoTestWrite; User = sa; Password = P@ssw0rd1; MultipleActiveResultSets = True";
            await TestDbTools.Delete(connectionString, "Journal");
            
            return await scenario.Run()
                           .Node(new DomainConfiguration(new BalloonDomainConfiguration(),
                                                         new ProgrammerAggregateDomainConfiguration()),
                                                         Logger,
                                                         connectionString);
        }

        public AggregateScenarioNodePersistenceTests(ITestOutputHelper output) : base(output) { }
    }
}