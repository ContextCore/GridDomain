using System.Threading.Tasks;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Serilog;

namespace GridDomain.Tests.Scenarios.Runners
{
    public class AggregateScenarioNodeRunner<TAggregate> : IAggregateScenarioRunner<TAggregate> where TAggregate : class, IAggregate
    {
        private readonly IExtendedGridDomainNode _extendedGridDomainNode;

        public AggregateScenarioNodeRunner(IExtendedGridDomainNode node)
        {
            _extendedGridDomainNode = node;
        }

        public async Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario scenario)
        {
            var eventsRepository = new ActorSystemEventRepository(_extendedGridDomainNode.System);
            var aggregateRepository = new AggregateRepository(eventsRepository, _extendedGridDomainNode.EventsAdaptersCatalog);

            await eventsRepository.Save(scenario.AggregateId);

            foreach (var command in scenario.GivenCommands)
            {
                await _extendedGridDomainNode.Execute(command);
            }

            var events = await eventsRepository.Load(scenario.AggregateId);
            var aggregate = await aggregateRepository.LoadAggregate<TAggregate>(scenario.AggregateId);

            return new AggregateScenarioRun<TAggregate>(scenario, aggregate, events, Log);
        }

        public ILogger Log => _extendedGridDomainNode.Log;
    }
}