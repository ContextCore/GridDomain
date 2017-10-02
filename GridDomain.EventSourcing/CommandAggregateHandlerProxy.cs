using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing {
    public class CommandAggregateHandlerProxy<T> : IAggregateCommandsHandler<T> where T : CommandAggregate
    {
        private readonly CommandAggregate _aggregate;

        public CommandAggregateHandlerProxy(CommandAggregate aggregate)
        {
            _aggregate = aggregate;
        }

        public Task ExecuteAsync(T aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            return _aggregate.CommandsHandler.ExecuteAsync(aggregate, command, persistenceDelegate);
        }

        public IReadOnlyCollection<Type> RegisteredCommands => _aggregate.RegisteredCommands;
        public Type AggregateType => _aggregate.AggregateType;
    }
}