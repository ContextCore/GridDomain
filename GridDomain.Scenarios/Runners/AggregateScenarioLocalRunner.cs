using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Aggregates;
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
            var aggregate = scenario.Configuration.AggregateFactory.Build(scenario.AggregateId);

            foreach (var evt in scenario.GivenEvents)
            {
                aggregate.Apply(evt);
            }

            IReadOnlyCollection<IDomainEvent> producedEvents = null;
            //When
            foreach (var cmd in scenario.GivenCommands)
            {
                try
                {
                    producedEvents = await aggregate.Execute(cmd);

                    foreach (var e in producedEvents)
                    {
                        if(e.Version == aggregate.Version)
                            aggregate.Apply(e);
                        else if (e.Version > aggregate.Version)
                            throw new AggregateVersionMismatchException(); 
                    }
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