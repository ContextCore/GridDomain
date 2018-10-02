using System.Threading.Tasks;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Scenarios.Builders;
using GridDomain.Tests.Unit;
using Serilog.Core;
using Xunit.Abstractions;

namespace GridDomain.Scenarios.Tests 
{
    public  class ScenarioRunnerTest
    {
        protected readonly Logger Logger;

        protected ScenarioRunnerTest(ITestOutputHelper output)
        {
            Logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        protected virtual Task<IAggregateScenarioRun<T>> Run<T>(
            IAggregateScenario<T> scenario) where T : class, IAggregate
        {
            return null;
        }

        protected Task<IAggregateScenarioRun<T>> Run<T>(
            IAggregateScenarioBuilder<T> scenarioBuilder) where T : class, IAggregate
        {
            return Run(scenarioBuilder.Build());
        }

//        protected async Task Check(IAggregateScenario<BinaryOptionGame> scenario)
//        {
//            await Run(scenario).Check();
//        }
    }
}