using System.Linq;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Tests.Scenarios;
using GridDomain.Tests.Scenarios.Runners;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Serilog.Core;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Scenarios
 {
     public class AggregateScenarioClusterTests
     {

         private readonly Logger _log;
         private ITestOutputHelper _testOutputHelper;

         public AggregateScenarioClusterTests(ITestOutputHelper output)
         {
             _testOutputHelper = output;
             _log = new XUnitAutoTestLoggerConfiguration(output,LogEventLevel.Verbose,nameof(AggregateScenarioClusterTests)).CreateLogger();
         }

         [Fact]
         public async Task When_defined_aggregate_handler_then_it_can_execute_commands_and_produce_events_with_node_runner()
         {
             var aggregateId = "my_balloon";

             var run = await new AggregateScenarioBuilder()
                             .When(new InflateNewBallonCommand(42, aggregateId))
                             .Then(new BalloonCreated("42", aggregateId))
                             .Run
                             .Cluster<Balloon>(new BalloonDomainConfiguration(), 
                                               () => new XUnitAutoTestLoggerConfiguration(_testOutputHelper,LogEventLevel.Verbose,nameof(AggregateScenarioClusterTests)));

             var producedAggregate = run.Aggregate;

             //aggregate is changed 
             Assert.Equal("42", producedAggregate.Title);
             Assert.Equal(aggregateId, producedAggregate.Id);

             //event is produced and stored
             var producedEvent = run.ProducedEvents.OfType<BalloonCreated>()
                                    .First();
             Assert.Equal("42", producedEvent.Value);

             //scenario check is OK
             run.Check();
         }
     }
 }