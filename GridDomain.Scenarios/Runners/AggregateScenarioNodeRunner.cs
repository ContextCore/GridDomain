using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Serilog;

namespace GridDomain.Scenarios.Runners
{
    public class AggregateScenarioNodeRunner<TAggregate> : IAggregateScenarioRunner<TAggregate> where TAggregate : class, IAggregate
    {
        private readonly IExtendedGridDomainNode _extendedGridDomainNode;
        
        public AggregateScenarioNodeRunner(IExtendedGridDomainNode node)
        {
            _extendedGridDomainNode = node;
        }

        public async Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario<TAggregate> scenario)
        {
            var eventsRepository = new ActorSystemEventRepository(_extendedGridDomainNode.System);
            var aggregateRepository = new AggregateRepository(eventsRepository, _extendedGridDomainNode.EventsAdaptersCatalog);

            if(scenario.GivenEvents.Any())
            {
                await eventsRepository.Save<TAggregate>(scenario.AggregateId,scenario.GivenEvents.ToArray());
            }

            foreach (var command in scenario.GivenCommands)
            {
                await _extendedGridDomainNode.Execute(command);
            }

            var events = await eventsRepository.Load<TAggregate>(scenario.AggregateId);
            var aggregate = await aggregateRepository.LoadAggregate<TAggregate>(scenario.AggregateId,scenario.Dependencies.AggregateFactory);

            return new AggregateScenarioRun<TAggregate>(scenario, aggregate, events, Log);
        }

        public ILogger Log => _extendedGridDomainNode.Log;
    }
}