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
using Xunit;

namespace GridDomain.Tests.Unit.Scenario
{
    public class ProcessScenarioTests
    {
        [Fact]
        public async Task Process_scenario_transit_on_events_respecting_giving_state()
        {
            var personId = Guid.NewGuid();
            var coffeMachineId = Guid.NewGuid();
            var sofaId = Guid.NewGuid();
            var initialSofaId = Guid.NewGuid();

            var scenario = ProcessScenario.New<SoftwareProgrammingState>(new SoftwareProgrammingProcess(), new SoftwareProgrammingProcessStateFactory());
            var results = await scenario.Given(new SoftwareProgrammingState(Guid.NewGuid(), nameof(SoftwareProgrammingProcess.MakingCoffee)){SofaId = initialSofaId})
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
    }
}