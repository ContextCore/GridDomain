using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Aggregates;
using Microsoft.Extensions.Logging;

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

            var planRuns = new List<PlanRun>();
            foreach (var plan in scenario.Plans)
            {
                var producedEvents = new List<IDomainEvent>();
                Log.LogInformation("running step {step}", plan.Step);
                //When
                foreach (var cmd in plan.GivenCommands)
                {
                    IReadOnlyCollection<IDomainEvent> events;
                    try
                    {
                        events = await aggregate.Execute(cmd);

                    }
                    catch (Exception ex)
                    {
                        Log.LogError("failed to execute an aggregate command {cmd}");
                        throw;
                    }

                    producedEvents.AddRange(events);

                    foreach (var e in events)
                    {
                        if (e.Version == aggregate.Version)
                            aggregate.Apply(e);
                        else
                        {
                            Log.LogError("Expected version is {version}. Fact version is {fact} on event {evt}",
                                aggregate.Version, e.Version, e);
                            throw new AggregateVersionMismatchException();
                        }
                    }
                }
                planRuns.Add(new PlanRun(plan, producedEvents));
            }

            //Then
            return new AggregateScenarioRun<TAggregate>(scenario, aggregate, planRuns, Log);
        }
    }
}