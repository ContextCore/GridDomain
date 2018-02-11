using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Remote;
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
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Scenario
{
    public class ProcessScenarioTests
    {
        public ProcessScenarioTests(ITestOutputHelper output)
        {
            Log.Logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }
        [Fact]
        public async Task Process_scenario_transit_on_events_respecting_giving_state()
        {
            var personId = Guid.NewGuid().ToString();
            var coffeMachineId = Guid.NewGuid().ToString();
            var sofaId = Guid.NewGuid().ToString();
            var initialSofaId = Guid.NewGuid().ToString();

            var scenario = ProcessScenario.New<SoftwareProgrammingState>(new SoftwareProgrammingProcess(), new SoftwareProgrammingProcessStateFactory());
            var results = await scenario.Given(new SoftwareProgrammingState(Guid.NewGuid().ToString(), nameof(SoftwareProgrammingProcess.MakingCoffee)){SofaId = initialSofaId})
                                        .When(new CoffeMakeFailedEvent(coffeMachineId,personId),
                                              new SleptWellEvent(personId,sofaId))
                                        .Then(new GoSleepCommand(personId, initialSofaId))
                                        .Run();

            
            Assert.Single(results.ExpectedCommands);
            Assert.Equal(2,results.ReceivedEvents.Length);
            Assert.Equal(sofaId, results.State.SofaId);
            results.CheckStateName(nameof(SoftwareProgrammingProcess.Coding));
            results.CheckProducedCommands();
        }

        [Fact]
        public async Task Process_scenario_without_state_creates_it_on_start_message()
        {
            var personId = "Andrey";
            var coffeMachineId = "Aulika Top";
            await ProcessScenario.New(new SoftwareProgrammingProcess(), new SoftwareProgrammingProcessStateFactory(coffeMachineId))
                                 .When(new GotTiredEvent(personId))
                                 .Then(new MakeCoffeCommand(personId, coffeMachineId))
                                 .Run()
                                 .CheckProducedCommands();
        }
    }
}