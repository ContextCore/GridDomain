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
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using KellermanSoftware.CompareNetObjects;
using Xunit;

namespace GridDomain.Tests.Unit.Scenario
{
    public class ProcessScenarioTests
    {
        [Fact]
        public async Task Process_scenario_transit_on_events_respecting_giving_state()
        {
           // var calculator = new InMemoryPriceCalculator();
           // var factory = new BuyNowProcessManagerFactory(calculator, Log.Logger);
           // var scenario = ProcessScenario.New<BuyNow, BuyNowState>(BuyNow.Descriptor, factory);
           // calculator.Add(state.SkuId, new Money(100));
           //
           // var state = scenario.NewState(nameof(BuyNow.CreatingOrder), c => c.Without(d => d.ReserveId));
           //
           //
           // await scenario.Given(state)
           //               .When(new OrderCreated(state.OrderId, 123, state.UserId, OrderStatus.Created))
           //               .Then(
           //                     new AddItemToOrderCommand(state.OrderId,
           //                                               state.SkuId,
           //                                               state.Quantity,
           //                                               await calculator.CalculatePrice(state.SkuId, state.Quantity)))
           //               .Run()
           //               .CheckProducedCommands()
           //               .CheckOnlyStateNameChanged(nameof(BuyNow.AddingOrderItems));
           //
           //
           // ProcessScenario.New<SoftwareProgrammingProcess,SoftwareProgrammingState>()
           //     .
            await Task.CompletedTask;
        }

        [Fact]
        public async Task Process_scenario_without_state_creates_it_on_start_message()
        {
            var personId = Guid.NewGuid();
            var coffeMachineId = Guid.NewGuid();
            await ProcessScenario.New(new SoftwareProgrammingProcess(), new SoftwareProgrammingProcessStateFactory(coffeMachineId))
                                 .When(new GotTiredEvent(personId))
                                 .Then(new MakeCoffeCommand(personId, coffeMachineId))
                                 .Run()
                                 .CheckProducedCommands();
        }
    }
}