using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Scenarios;
using GridDomain.Scenarios.Builders;
using GridDomain.Scenarios.Runners;
using GridDomain.Scenarios.Tests;
using Serilog;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Scenarios.Tests
{
    public static class XUnitAssertExtensions
    {
        public static async Task<TEx> ShouldThrow<TEx>(this Task t, Predicate<TEx> predicate = null)
            where TEx : Exception
        {
            try
            {
                await t;
            }
            catch (Exception ex)
            {
                return CheckException(ex, predicate);
            }

            throw new ExpectedExceptionWasNotRaisedException();
        }

        public class ExpectedExceptionWasNotRaisedException : Exception
        {
        }

        private static TEx CheckException<TEx>(Exception ex, Predicate<TEx> predicate = null) where TEx : Exception
        {
            var exception = ex.UnwrapSingle();
            if (!(exception is TEx))
                throw new ExpectedExceptionWasNotRaisedException();

            if (predicate != null && !predicate((TEx) exception))
                throw new PredicateNotMatchException();
            return (TEx) exception;
        }

        public class PredicateNotMatchException : Exception
        {
        }
    }

    public static class ExceptionExtensions
    {
        public static Exception UnwrapSingle(this AggregateException aggregateException)
        {
            if (aggregateException == null)
                return null;

            if (aggregateException.InnerExceptions.Count > 1)
                return aggregateException;

            if (aggregateException.InnerExceptions.Count == 0)
            {
                //for cases when inner exceptions were lost due to hyperion serializer
                return aggregateException.InnerException ?? aggregateException;
            }

            //for cases when inner exceptions were lost due to hyperion serializer
            if (aggregateException.InnerException != null)
                return aggregateException.InnerException;

            return aggregateException.InnerExceptions.First().UnwrapSingle();
        }

        public static Exception UnwrapSingle(this Exception exeption)
        {
            if (exeption == null)
                return null;
            return !(exeption is AggregateException aggregateException)
                ? exeption
                : aggregateException.InnerException.UnwrapSingle();
        }
    }
}

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

public abstract class AggregateScenarioTests
{
    protected readonly Logger Logger;

    public AggregateScenarioTests(ITestOutputHelper output)
    {
        Logger = new LoggerConfiguration().WriteTo.XunitTestOutput(output).CreateLogger();
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

        var scenario = new AggregateScenarioBuilder<Cat>()
            .Name(nameof(When_defined_aggregate_handler_then_it_can_execute_commands_and_produce_events_with_builder
            ))
            .With(new AggregateDependencies<Cat>())
            .When(new Cat.GetNewCatCommand(catName))
            .Then(new Cat.Born(catName));

        var run = await Run(scenario);

        var producedAggregate = run.Aggregate;

        //aggregate is changed 
        Assert.Equal(catName, producedAggregate.Name);
        Assert.Equal(catName, producedAggregate.Id);

        //event is produced and stored
        var producedEvent = run.ProducedEvents.OfType<Cat.Born>().First();
        Assert.Equal(catName, producedEvent.Name);

        //scenario check is OK
        run.Check();
    }


    [Fact]
    public async Task When_defined_scenario_has_given_it_is_applied_even_without_command()
    {
        var aggregateId = "personA";

        var scenarioBuilder = AggregateScenario.New<Cat>()
            .Name(nameof(When_defined_scenario_has_given_it_is_applied_even_without_command))
            .Given(new Cat.Born(aggregateId));

        var scenario = await Run(scenarioBuilder);

        //aggregate is changed 
        Assert.Equal(aggregateId, scenario.Aggregate.Name);
        Assert.Equal(aggregateId, scenario.Aggregate.Id);

        //no events was produced
        Assert.Empty(scenario.ProducedEvents);
    }


    class FedCatFactory : IAggregateFactory<Cat>
    {
        private string _predifinedId;

        public FedCatFactory(string predifinedId)
        {
            _predifinedId = predifinedId;
        }

        public Cat Build(string id = null)
        {
            var cat = new Cat();
            cat.Apply(new Cat.Born(_predifinedId));
            cat.Apply(new Cat.Feeded(_predifinedId));
            return cat;
        }
    }

    [Fact]
    public async Task Given_factory_When_defined_scenario_has_given_it_is_applied_even_without_command()
    {
        var aggregateId = "Bonifacii";

        var scenario = AggregateScenario.New(new AggregateDependencies<Cat>(new FedCatFactory(aggregateId)))
            .Name(nameof(Given_factory_When_defined_scenario_has_given_it_is_applied_even_without_command));

        var run = await Run(scenario);
        //aggregate is changed 
        Assert.Equal(aggregateId, run.Aggregate.Name);
        Assert.Equal(aggregateId, run.Aggregate.Id);
        Assert.Equal(Mood.Good, run.Aggregate.Mood);

        //no events was produced
        Assert.Empty(run.ProducedEvents);
    }

    [Fact]
    public async Task When_defined_scenario_it_checks_for_produced_events_properties()
    {
        var aggregateId = "Mailk";

        var builder = AggregateScenario.New<Cat>()
            .Name(nameof(When_defined_scenario_it_checks_for_produced_events_properties))
            .When(new Cat.GetNewCatCommand(aggregateId))
            .Then(new Cat.Born(aggregateId));


        var local = Run(builder);

        await local
            .Check()
            .ShouldThrow<ProducedEventsDifferException>();
    }

    [Fact]
    public async Task When_defined_scenario_it_checks_for_produced_events_count()
    {
        var aggregateId = "Radja";

        var aggregateScenarioBuilder = AggregateScenario.New<Cat>()
            .Name(nameof(When_defined_scenario_it_checks_for_produced_events_count))
            .When(new Cat.GetNewCatCommand(aggregateId))
            .Then(new Cat.Born(aggregateId),
                new Cat.Feeded(aggregateId));

        var local = Run(aggregateScenarioBuilder);
        await local
            .Check()
            .ShouldThrow<ProducedEventsCountMismatchException>();
    }


    class WrongCommand : Command<Cat>
    {
        public WrongCommand(string aggregateId) : base(aggregateId)
        {
        }
    }
    
        [Fact]
        public async Task When_defined_scenario_try_execute_missing_command_it_throws_exception()
        {
            var aggregateId = "test_aggregate";

            var aggregateScenarioBuilder = AggregateScenario.New<Cat>()
                .Name(nameof(When_defined_scenario_try_execute_missing_command_it_throws_exception))
                .When(new WrongCommand(aggregateId));
            var run = Run(aggregateScenarioBuilder);

            await run
                  .Check()
                  .ShouldThrow<UnknownCommandException>();
        }

    [Fact]
    public async Task When_defined_scenario_executes_command_with_exception_it_throws_command_exception()
    {
        var name = "Circle";
        var aggregateScenarioBuilder = AggregateScenario.New<Cat>()
            .Name(nameof(When_defined_scenario_executes_command_with_exception_it_throws_command_exception))
            .When(new Cat.GetNewCatCommand(name),
                  new Cat.PetCommand(name));

        var local = Run(aggregateScenarioBuilder);
        await local
            .Check()
            .ShouldThrow<Cat.IsUnhappyException>();
    }
}