using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Serilog;

namespace GridDomain.Scenarios.Runners {
    
    
    
    
    
    public class AggregateScenarioClusterInMemoryRunner<TAggregate> : IAggregateScenarioRunner<TAggregate> where TAggregate : class, IAggregate
    {
        private readonly IExtendedGridDomainNode[] _nodes;

        public AggregateScenarioClusterInMemoryRunner(IExtendedGridDomainNode[] nodes)
        {
            _nodes = nodes;
        }

        public async Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario<TAggregate> scenario)
        {
           
            
            if(scenario.GivenEvents.Any())
            {
                //naive save to in-memory journal for each node
                //as we don't know what exact node will execute our command
                foreach (var node in _nodes)
                {
                    using (var eventsRepository = new ActorSystemEventRepository(node.System))
                        await eventsRepository.Save<TAggregate>(scenario.AggregateId,scenario.GivenEvents.ToArray());

                }
            }

            foreach (var command in scenario.GivenCommands)
            {
                await _nodes.First().Execute(command);
            }

            //for cases when we did not execute any commands in scenario
            //than we need return aggregate build only from given events
            
            TAggregate aggregate = null;
            foreach (var node in _nodes)
            {
                //Will naive search in all nodes in-memory journals 
                using (var eventsRepository = new ActorSystemEventRepository(node.System))
                {
                    var events = (await eventsRepository.Load<TAggregate>(scenario.AggregateId)).Skip(scenario.GivenEvents.Count).ToArray();
                    var aggregateRepository = new AggregateRepository(eventsRepository, node.EventsAdaptersCatalog);
                    aggregate = await aggregateRepository.LoadAggregate<TAggregate>(scenario.AggregateId, scenario.Dependencies.AggregateFactory);
                    
                    if (!events.Any()) continue;
                   
                    return new AggregateScenarioRun<TAggregate>(scenario, aggregate, events, Log);
                }
            }

            return new AggregateScenarioRun<TAggregate>(scenario, aggregate,new DomainEvent[]{}, Log);
        }

        public class CannotFindAggregateException : Exception { }
        public class CannotSaveEventsInClusterWithoutPersistenceException : Exception { }


        public ILogger Log => _nodes.First().Log;
    }
}