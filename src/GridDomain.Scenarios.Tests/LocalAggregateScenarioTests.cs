using System.Threading.Tasks;
using GridDomain.Scenarios.Runners;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;

namespace GridDomain.Scenarios.Tests
{
    public class LocalAggregateScenarioTests : AggregateScenarioTests
    {
        private SerilogLoggerProvider _provider;

        public LocalAggregateScenarioTests(ITestOutputHelper output) : base(output)
        {
            var cfg = new LoggerConfiguration().WriteTo.TestOutput(output).CreateLogger();
            _provider = new Serilog.Extensions.Logging.SerilogLoggerProvider(cfg);
        }

        protected override Task<IAggregateScenarioRun<T>> Run<T>(IAggregateScenario<T> scenario)
        {
            return scenario.Run().Local(_provider);
        }
    }
}