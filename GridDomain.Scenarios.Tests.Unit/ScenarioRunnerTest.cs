using System.Threading.Tasks;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Scenarios;
using GridDomain.Scenarios.Builders;
using Serilog.Core;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Scenario {
    public abstract class ScenarioRunnerTest
    {
        protected readonly Logger Logger;
        //   protected virtual IAggregateScenarioBuilder<T> Scenario { get; }

        protected ScenarioRunnerTest(ITestOutputHelper output)
        {
            Logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        protected abstract Task<IAggregateScenarioRun<T>> Run<T>(
            IAggregateScenario<T> scenario) where T : class, IAggregate;

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