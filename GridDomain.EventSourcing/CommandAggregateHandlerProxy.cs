using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing {
    public class CommandAggregateHandlerProxy<T> : IAggregateCommandsHandler<T> where T : CommandAggregate
    {
        private readonly AggregateCommandsHandler<CommandAggregate> _aggregateCommandsHandler;

        public CommandAggregateHandlerProxy(CommandAggregate aggregate)
        {
            _aggregateCommandsHandler = aggregate.CommandsRouter;
            RegisteredCommands = aggregate.RegisteredCommands;
        }

        public Task ExecuteAsync(T aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            return _aggregateCommandsHandler.ExecuteAsync(aggregate, command, persistenceDelegate);
        }

        public IReadOnlyCollection<Type> RegisteredCommands { get; }
        public Type AggregateType { get; } = typeof(T);
    }
}