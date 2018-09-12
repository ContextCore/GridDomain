using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
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
            var aggregate = scenario.Dependencies.AggregateFactory
                                    .Build<TAggregate>(scenario.AggregateId);


            foreach (var evt in scenario.GivenEvents)
            {
                aggregate.Apply(evt);
            }

            IReadOnlyCollection<DomainEvent> producedEvents = null;
            //When
            foreach (var cmd in scenario.GivenCommands)
            {
                try
                {
                    producedEvents = await aggregate.Execute(cmd);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "failed to execute an aggregate command");
                    throw;
                }
            }

            //Then
            return new AggregateScenarioRun<TAggregate>(scenario, aggregate, producedEvents, Log);
        }
    }
}