using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Scenarios.Runners
{
    public class AggregateScenarioLocalRunner<TAggregate> : IAggregateScenarioRunner<TAggregate> where TAggregate : class, IAggregate
    {
        private TAggregate _aggregate;
        public ILogger Log { get; }

        public AggregateScenarioLocalRunner(TAggregate aggregate, IAggregateCommandsHandler<TAggregate> handler, ILogger log)
        {
            CommandsHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            _aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
            Log = log;
        }

        private IAggregateCommandsHandler<TAggregate> CommandsHandler { get; }

        public async Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario scenario)
        {
            foreach (var evt in scenario.GivenEvents)
            {
                _aggregate.Apply(evt);
            }

            //When
            foreach (var cmd in scenario.GivenCommands)
            {
                try
                {
                    _aggregate = await CommandsHandler.ExecuteAsync(_aggregate, cmd);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "failed to execute an aggregate command");
                    throw;
                }
            }

            //Then
            var producedEvents = _aggregate.GetUncommittedEvents()
                                           .ToArray();
            _aggregate.ClearUncommitedEvents();
            return new AggregateScenarioRun<TAggregate>(scenario, _aggregate, producedEvents, Log);
        }
    }
}