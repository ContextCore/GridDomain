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
    public class ProcessScenarioTests
    {
        [Fact]
        public async Task Process_scenario_transit_on_events_respecting_giving_state()
        {
            var calculator = new InMemoryPriceCalculator();
            var factory = new BuyNowProcessManagerFactory(calculator, Log.Logger);
            var scenario = ProcessScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, factory);
            calculator.Add(state.SkuId, new Money(100));

            var state = scenario.NewState(nameof(BuyNow.CreatingOrder), c => c.Without(d => d.ReserveId));


            await scenario.Given(state)
                          .When(new OrderCreated(state.OrderId, 123, state.UserId, OrderStatus.Created))
                          .Then(
                                new AddItemToOrderCommand(state.OrderId,
                                                          state.SkuId,
                                                          state.Quantity,
                                                          await calculator.CalculatePrice(state.SkuId, state.Quantity)))
                          .Run()
                          .CheckProducedCommands()
                          .CheckOnlyStateNameChanged(nameof(BuyNow.AddingOrderItems));


            ProcessScenario.New<SoftwareProgrammingProcess,SoftwareProgrammingState>()
                .
        }
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
                                                  .Then(new PersonCreated(aggregateId, aggregateId))
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
                                   .ShouldThrow<CannotFindAggregateCommandHandlerExeption>();
        }
    }
}