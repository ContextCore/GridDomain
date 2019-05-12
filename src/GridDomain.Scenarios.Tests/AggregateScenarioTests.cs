using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Scenarios;
using GridDomain.Scenarios.Builders;
using GridDomain.Scenarios.Runners;
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Scenarios.Tests
{
    public abstract class AggregateScenarioTests
    {
        protected readonly Logger Logger;

        public AggregateScenarioTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration().WriteTo.TestOutput(output).CreateLogger();
        }

        protected abstract Task<IAggregateScenarioRun<T>> Run<T>(
            IAggregateScenario<T> scenario) where T : class, IAggregate;

        protected Task<IAggregateScenarioRun<T>> Run<T>(
            IAggregateScenarioBuilder<T> scenarioBuilder) where T : class, IAggregate
        {
            return Run(scenarioBuilder.Build());
        }

        [Fact]
        public async Task When_defined_aggregate_handler_then_it_can_execute_commands_and_produce_events_with_builder()
        {
            var catName = "Bonifacii";

            var scenario = new AggregateScenarioBuilder<Dog>()
                .Name(nameof(When_defined_aggregate_handler_then_it_can_execute_commands_and_produce_events_with_builder
                ))
                .With(new AggregateConfiguration<Dog>())
                .When(new Dog.GetNewDogCommand(catName))
                .Then(new Dog.Born(catName, 0));

            var run = await Run(scenario);

            var producedAggregate = run.Aggregate;

            //aggregate is changed 
            Assert.Equal(catName, producedAggregate.Name);
            Assert.Equal(catName, producedAggregate.Id);

            //event is produced and stored
            var producedEvent = run.Produced.First().ProducedEvents.OfType<Dog.Born>().First();
            Assert.Equal(catName, producedEvent.Name);

            //scenario check is OK
            run.Check();
        }


        [Fact]
        public async Task When_defined_scenario_has_given_it_is_applied_even_without_command()
        {
            var aggregateId = "personA";

            var scenarioBuilder = AggregateScenario.New<Dog>()
                .Name(nameof(When_defined_scenario_has_given_it_is_applied_even_without_command))
                .Given(new Dog.Born(aggregateId, 0));

            var scenario = await Run(scenarioBuilder);

            //aggregate is changed 
            Assert.Equal(aggregateId, scenario.Aggregate.Name);
            Assert.Equal(aggregateId, scenario.Aggregate.Id);

            //no events was produced
            Assert.Empty(scenario.Produced);
        }


        class FedCatFactory : IAggregateFactory<Dog>
        {
            private string _predifinedId;

            public FedCatFactory(string predifinedId)
            {
                _predifinedId = predifinedId;
            }

            public Dog Build(string id = null)
            {
                var cat = new Dog();
                cat.Apply(new Dog.Born(_predifinedId, 0));
                cat.Apply(new Dog.Feeded(_predifinedId, 1));
                return cat;
            }
        }

        [Fact]
        public async Task Given_factory_When_defined_scenario_has_given_it_is_applied_even_without_command()
        {
            var aggregateId = "Bonifacii";

            var scenario = AggregateScenario.New(new AggregateConfiguration<Dog>(new FedCatFactory(aggregateId)))
                .Name(nameof(Given_factory_When_defined_scenario_has_given_it_is_applied_even_without_command));

            var run = await Run(scenario);
            //aggregate is changed 
            Assert.Equal(aggregateId, run.Aggregate.Name);
            Assert.Equal(aggregateId, run.Aggregate.Id);
            Assert.Equal(Mood.Good, run.Aggregate.Mood);

            //no events was produced
            Assert.Empty(run.Produced);
        }

        [Fact]
        public async Task When_defined_scenario_it_checks_for_produced_events_properties()
        {
            var builder = AggregateScenario.New<Dog>()
                .Name(nameof(When_defined_scenario_it_checks_for_produced_events_properties))
                .When(new Dog.GetNewDogCommand("Mailk"))
                .Then(new Dog.Born("wrongName", 0));


            var local = Run(builder);

            await local
                .Check()
                .ShouldThrow<ProducedEventsDifferException>();
        }

        [Fact]
        public async Task When_defined_scenario_it_checks_for_produced_events_count()
        {
            var aggregateId = "Radja";

            var aggregateScenarioBuilder = AggregateScenario.New<Dog>()
                .Name(nameof(When_defined_scenario_it_checks_for_produced_events_count))
                .When(new Dog.GetNewDogCommand(aggregateId))
                .Then(new Dog.Born(aggregateId, 0),
                    new Dog.Feeded(aggregateId, 1));

            var local = Run(aggregateScenarioBuilder);
            await local
                .Check()
                .ShouldThrow<ProducedEventsCountMismatchException>();
        }


        class WrongCommand : Command<Dog>
        {
            public WrongCommand(string aggregateId) : base(aggregateId)
            {
            }
        }

        [Fact]
        public async Task When_defined_scenario_try_execute_missing_command_it_throws_exception()
        {
            var aggregateId = "test_aggregate";

            var aggregateScenarioBuilder = AggregateScenario.New<Dog>()
                .Name(nameof(When_defined_scenario_try_execute_missing_command_it_throws_exception))
                .When(new WrongCommand(aggregateId));
            var run = Run(aggregateScenarioBuilder);

            await run
                .Check()
                .ShouldThrow<UnknownCommandException>();
        }

        [Fact]
        public async Task Given_two_commands_When_executing_both_Then_produced_events_accumulated()
        {
            var name = "Circle";
            var aggregateScenarioBuilder = AggregateScenario.New<Dog>()
                .Name(nameof(Given_two_commands_When_executing_both_Then_produced_events_accumulated))
                .When(new Dog.GetNewDogCommand(name),
                      new Dog.PetCommand(name))
                .Then(new Dog.Born(name, 0),
                      new Dog.Tired(name, 1));

            var local = Run(aggregateScenarioBuilder);
            await local
                .Check()
                .ShouldThrow<Dog.IsUnhappyException>();
        }
        
        [Fact]
        public async Task Given_several_when_then_When_executing_Then_produced_events_accumulated()
        {
            var name = "Circle";
            var aggregateScenarioBuilder = AggregateScenario.New<Dog>()
                .Name(nameof(Given_two_commands_When_executing_both_Then_produced_events_accumulated))
                .When(new Dog.GetNewDogCommand(name),
                      new Dog.FeedCommand(name),
                      new Dog.PetCommand(name))
                .Then(new Dog.Born(name, 0),
                      new Dog.Feeded(name, 1),
                      new Dog.Tired(name, 2));

            var local = Run(aggregateScenarioBuilder);
            await local.Check();
        }
        
        [Fact]
        public async Task Given_mixed_when_then_When_executing_Then_pairs_checked_together()
        {
            var name = "Circle";
            var aggregateScenarioBuilder = AggregateScenario.New<Dog>()
                .Name(nameof(Given_two_commands_When_executing_both_Then_produced_events_accumulated))
                .When(new Dog.GetNewDogCommand(name))
                .Then(new Dog.Born(name, 0)) 
                .When(new Dog.FeedCommand(name))
                .Then(new Dog.Feeded(name, 1))
                .And()
                .When(new Dog.PetCommand(name))
                .Then(new Dog.Tired(name, 2));

            var local = Run(aggregateScenarioBuilder);
            await local.Check();
        }

        [Fact]
        public async Task When_defined_scenario_executes_command_with_exception_it_throws_command_exception()
        {
            var name = "Circle";
            var aggregateScenarioBuilder = AggregateScenario.New<Dog>()
                .Name(nameof(When_defined_scenario_executes_command_with_exception_it_throws_command_exception))
                .When(new Dog.GetNewDogCommand(name),
                    new Dog.PetCommand(name));

            var local = Run(aggregateScenarioBuilder);
            await local
                .Check()
                .ShouldThrow<Dog.IsUnhappyException>();
        }
    }
}