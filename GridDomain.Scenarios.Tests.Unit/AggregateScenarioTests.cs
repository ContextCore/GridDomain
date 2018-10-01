using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.Scenarios;
using GridDomain.Scenarios.Builders;
using GridDomain.Scenarios.Runners;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.Unit.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Remotion.Linq.Clauses.ResultOperators;
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Scenario
{
    public abstract class AggregateScenarioTests : ScenarioRunnerTest
    {

        public AggregateScenarioTests(ITestOutputHelper output) : base(output) { }

      
        [Fact]
        public async Task When_defined_aggregate_handler_then_it_can_execute_commands_and_produce_events_with_builder()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();

            var scenario = new AggregateScenarioBuilder<Balloon>()
                           .With(new BalloonDependencies())
                           .When(new InflateNewBallonCommand(42, aggregateId))
                           .Then(new BalloonCreated("42", aggregateId));

            var run = await Run(scenario);

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

        [Fact]
        public async Task When_defined_aggregate_handler_then_it_can_execute_commands_and_produce_events_with_explicit_runner()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();

            var scenario = AggregateScenario.New<Balloon>()
                                            .With(new BalloonDependencies())
                                            .When(new InflateNewBallonCommand(42, aggregateId))
                                            .Then(new BalloonCreated("42", aggregateId));

            var run = await Run(scenario);
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

        [Fact]
        public async Task When_defined_aggregate_handler_then_it_can_execute_commands_and_produce_events()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();


            var scenario = AggregateScenario.New<Balloon>()
                                            .With(new BalloonDependencies())
                                            .When(new InflateNewBallonCommand(42, aggregateId))
                                            .Then(new BalloonCreated("42", aggregateId));

            var run = await Run(scenario);

            //aggregate is changed 
            Assert.Equal("42", run.Aggregate.Title);
            Assert.Equal(aggregateId, run.Aggregate.Id);

            //event is produced and stored
            var producedEvent = run.ProducedEvents.OfType<BalloonCreated>()
                                   .First();
            Assert.Equal("42", producedEvent.Value);

            //scenario check is OK
            run.Check();
        }

        [Fact]
        public async Task Future_events_aggregate_can_be_tested()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();

            var scenario = AggregateScenario.New<Balloon>()
                                            .With(new BalloonDependencies())
                                            .When(new InflateNewBallonCommand(42, aggregateId))
                                            .Then(new BalloonCreated("42", aggregateId));
            var run = await Run(scenario);

            //aggregate is changed 
            Assert.Equal("42", run.Aggregate.Title);
            Assert.Equal(aggregateId, run.Aggregate.Id);

            //event is produced and stored
            var producedEvent = run.ProducedEvents.OfType<BalloonCreated>()
                                   .First();
            Assert.Equal("42", producedEvent.Value);

            //scenario check is OK
            run.Check();
        }

        [Fact]
        public async Task When_defined_scenario_has_given_it_is_applied_even_without_command()
        {
            var aggregateId = "personA";

            var scenarioBuilder = AggregateScenario.New<ProgrammerAggregate>()
                                                   .Given(new PersonCreated(aggregateId, aggregateId));

            var scenario = await Run(scenarioBuilder);

            //aggregate is changed 
            Assert.Equal(aggregateId, scenario.Aggregate.PersonId);
            Assert.Equal(aggregateId, scenario.Aggregate.Id);

            //no events was produced
            Assert.Empty(scenario.ProducedEvents);
        }

        [Fact]
        public async Task When_aggregate_has_custom_dependency_factory_scenario_can_use_it()
        {
            var aggregateId = "personA";
            var factory = AggregateDependencies.ForCommandAggregate<ProgrammerAggregate>();
            var scenarioBuilder = AggregateScenario.New<ProgrammerAggregate>()
                                                   .With(factory)
                                                   .Given(new PersonCreated(aggregateId, aggregateId));

            var scenario = await Run(scenarioBuilder);
            //aggregate is changed 
            Assert.Equal(aggregateId, scenario.Aggregate.PersonId);
            Assert.Equal(aggregateId, scenario.Aggregate.Id);

            //no events was produced
            Assert.Empty(scenario.ProducedEvents);
        }

        [Fact]
        public async Task Given_factory_When_defined_scenario_has_given_it_is_applied_even_without_command()
        {
            var aggregateId = "personA";

            var scenario = AggregateScenario.New<ProgrammerAggregate>()
                                            .With(new AggregateDependencies<ProgrammerAggregate>())
                                            .Given(new PersonCreated(aggregateId, aggregateId));

            var run = await Run(scenario);
            //aggregate is changed 
            Assert.Equal(aggregateId, run.Aggregate.PersonId);
            Assert.Equal(aggregateId, run.Aggregate.Id);

            //no events was produced
            Assert.Empty(run.ProducedEvents);
        }

        [Fact]
        public async Task When_defined_scenario_it_checks_for_produced_events_properties()
        {
            var aggregateId = "personA";

            var builder = AggregateScenario.New<Balloon>()
                                           .With(new BalloonDependencies())
                                           .When(new InflateNewBallonCommand(42, aggregateId))
                                           .Then(new BalloonCreated("420", aggregateId));


            var local = Run(builder);

            await local
                  .Check()
                  .ShouldThrow<ProducedEventsDifferException>();
        }

        [Fact]
        public async Task When_defined_scenario_it_checks_for_produced_events_count()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();

            var aggregateScenarioBuilder = AggregateScenario.New<Balloon>()
                                                            .With(new BalloonDependencies())
                                                            .When(new InflateNewBallonCommand(42, aggregateId))
                                                            .Then(new BalloonCreated("420", aggregateId),
                                                                  new BalloonTitleChanged("42", aggregateId));

            var local = Run(aggregateScenarioBuilder);
            await local
                  .Check()
                  .ShouldThrow<ProducedEventsCountMismatchException>();
        }

        [Fact]
        public async Task When_defined_scenario_try_execute_missing_command_on_default_handler_it_throws_exception()
        {
            var aggregateId = "test_aggregate";

            var aggregateScenarioBuilder = AggregateScenario.New<Balloon>()
                                                            .With(new BalloonDependencies())
                                                            .When(new CreateBalanceCommand(42, aggregateId))
                                                            .Then(new BalloonCreated("420", aggregateId),
                                                                  new BalloonTitleChanged("42", aggregateId));
            var run = Run(aggregateScenarioBuilder);

            await run
                  .Check()
                  .ShouldThrow<UnknownCommandExeption>();
        }

        [Fact]
        public async Task When_defined_scenario_executes_command_with_exception_it_throws_command_exception()
        {
            var aggregateId = Guid.NewGuid()
                                  .ToString();

            var aggregateScenarioBuilder = AggregateScenario.New<Balloon>()
                                                            .With(new BalloonDependencies())
                                                            .When(new PlanTitleWriteAndBlowCommand(43, aggregateId, TimeSpan.FromMilliseconds(50)));

            var local = Run(aggregateScenarioBuilder);
            await local
                  .Check()
                  .ShouldThrow((Predicate<BalloonException>) null);
        }
    }
}