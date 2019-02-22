using System.Threading.Tasks;
using GridDomain.Scenarios.Runners;
using Xunit.Abstractions;

namespace GridDomain.Scenarios.Tests
{
    public class LocalAggregateScenarioTests : AggregateScenarioTests
    {
        public LocalAggregateScenarioTests(ITestOutputHelper output) : base(output)
        {
        }

        protected override Task<IAggregateScenarioRun<T>> Run<T>(IAggregateScenario<T> scenario)
        {
            return scenario.Run().Local();
        }
    }
}