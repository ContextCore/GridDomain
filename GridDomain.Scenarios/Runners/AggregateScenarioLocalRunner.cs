using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Scenarios.Runners
{
    public class AggregateScenarioLocalRunner<TAggregate> : IAggregateScenarioRunner<TAggregate> where TAggregate : class, IAggregate
    {
        public ILogger Log { get; }

        public AggregateScenarioLocalRunner(ILogger log)
        {
            Log = log;
        }

        public async Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario<TAggregate> scenario)
        {
            var aggregate = scenario.Dependencies.CreateAggregateFactory()
                                    .Build<TAggregate>(scenario.AggregateId);

            var commandsHandler = scenario.Dependencies.CreateCommandsHandler();

            foreach (var evt in scenario.GivenEvents)
            {
                aggregate.Apply(evt);
            }

            //When
            foreach (var cmd in scenario.GivenCommands)
            {
                try
                {
                    aggregate = await commandsHandler.ExecuteAsync(aggregate, cmd);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "failed to execute an aggregate command");
                    throw;
                }
            }

            //Then
            var producedEvents = aggregate.GetUncommittedEvents()
                                          .ToArray();
            aggregate.ClearUncommitedEvents();
            return new AggregateScenarioRun<TAggregate>(scenario, aggregate, producedEvents, Log);
        }
    }
}