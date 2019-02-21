﻿using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Scenarios.Builders;
using Serilog;
using Serilog.Core;
using Xunit.Abstractions;

namespace GridDomain.Scenarios.Tests 
{
    public  class ScenarioRunnerTest
    {
        protected readonly Logger Logger;

        protected ScenarioRunnerTest(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration().WriteTo.XunitTestOutput(output).CreateLogger();
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