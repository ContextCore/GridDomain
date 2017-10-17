using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Xunit;

namespace GridDomain.Tests.Unit.Scenario
{
    public class AggregateScenarioTests
    {
        [Fact]
        public async Task When_defined_aggregate_handler_then_it_can_execute_commands_and_produce_events()
        {
            var aggregateId = Guid.NewGuid();
            var scenario = await AggregateScenario.New<Balloon, BalloonCommandHandler>()
                                                  .When(new InflateNewBallonCommand(42, aggregateId))
                                                  .Then(new BalloonCreated("42", aggregateId))
                                                  .Run();

            //aggregate is changed 
            Assert.Equal("42", scenario.Aggregate.Title);
            Assert.Equal(aggregateId, scenario.Aggregate.Id);

            //event is produced and stored
            var producedEvent = scenario.ProducedEvents.OfType<BalloonCreated>()
                                        .First();
            Assert.Equal("42", producedEvent.Value);

            //scenario check is OK
            scenario.Check();
        }


        [Fact]
        public async Task When_defined_scenario_has_given_it_is_applied_even_without_command()
        {
            var aggregateId = Guid.NewGuid();

            var scenario = await AggregateScenario.New<ProgrammerAggregate>()
                                                  .Given(new PersonCreated(aggregateId, aggregateId))
                                                  .Run();
            //aggregate is changed 
            Assert.Equal(aggregateId, scenario.Aggregate.PersonId);
            Assert.Equal(aggregateId, scenario.Aggregate.Id);

            //no events was produced
            Assert.Empty(scenario.ProducedEvents);

            //scenario check is OK
            scenario.Check();
        }

        [Fact]
        public async Task When_defined_scenario_it_checks_for_produced_events_properties()
        {
            var aggregateId = Guid.NewGuid();

            await AggregateScenario.New<Balloon, BalloonCommandHandler>()
                                   .When(new InflateNewBallonCommand(42, aggregateId))
                                   .Then(new BalloonCreated("420", aggregateId))
                                   .Run()
                                   .Check()
                                   .ShouldThrow<ProducedEventsDifferException>();
        }

        [Fact]
        public async Task When_defined_scenario_it_checks_for_produced_events_count()
        {
            var aggregateId = Guid.NewGuid();

            await AggregateScenario.New<Balloon, BalloonCommandHandler>()
                                   .When(new InflateNewBallonCommand(42, aggregateId))
                                   .Then(new BalloonCreated("420", aggregateId),
                                         new BalloonTitleChanged("42", aggregateId))
                                   .Run()
                                   .Check()
                                   .ShouldThrow<ProducedEventsCountMismatchException>();
        }


        [Fact]
        public async Task When_defined_scenario_try_execute_missing_command_on_default_handler_it_throws_exception()
        {
            var aggregateId = Guid.NewGuid();

            await AggregateScenario.New<Balloon, BalloonCommandHandler>()
                                   .When(new CreateBalanceCommand(42, aggregateId))
                                   .Then(new BalloonCreated("420", aggregateId),
                                         new BalloonTitleChanged("42", aggregateId))
                                   .Run()
                                   .Check()
                                   .CommandShouldThrow<CannotFindAggregateCommandHandlerExeption>();
        }

        [Fact]
        public async Task When_defined_scenario_executes_command_with_excpetion_it_throws_command_exception()
        {
            var aggregateId = Guid.NewGuid();

            await AggregateScenario.New<Balloon, BalloonCommandHandler>()
                                   .When(new PlanTitleWriteAndBlowCommand(43, aggregateId, TimeSpan.FromMilliseconds(50)))
                                   .Run()
                                   .Check()
                                   .CommandShouldThrow<BalloonException>();
        }
    }
}