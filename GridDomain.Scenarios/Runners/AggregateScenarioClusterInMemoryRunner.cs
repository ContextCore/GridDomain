using System;
using System.Linq;
using System.Threading.Tasks;
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
                throw new CannotSaveEventsInClusterWithoutPersistenceException();
            }

            foreach (var command in scenario.GivenCommands)
            {
                await _nodes.First().Execute(command);
            }

            foreach (var node in _nodes)
            {
                //Will naive search in all nodes in-memory journals 
                var eventsRepository = new ActorSystemEventRepository(node.System);
                var events = await eventsRepository.Load<TAggregate>(scenario.AggregateId);
                if (events.Any())
                {
                    var aggregateRepository = new AggregateRepository(eventsRepository, node.EventsAdaptersCatalog);
                    var aggregate = await aggregateRepository.LoadAggregate<TAggregate>(scenario.AggregateId);
                    return new AggregateScenarioRun<TAggregate>(scenario, aggregate, events, Log);
                }
            }

            throw new CannotFindAggregateException();
        }

        public class CannotFindAggregateException : Exception { }
        public class CannotSaveEventsInClusterWithoutPersistenceException : Exception { }


        public ILogger Log => _nodes.First().Log;
    }
}