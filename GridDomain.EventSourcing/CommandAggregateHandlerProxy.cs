using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing {
    public class CommandAggregateHandler<T> : IAggregateCommandsHandler<T> where T : CommandAggregate
    {

        public CommandAggregateHandler(CommandAggregate aggregate)
        {
            RegisteredCommands = aggregate.RegisteredCommands;
            if (!RegisteredCommands.Any())
                throw new MissingRegisteredCommandsException();
        }

        public Task ExecuteAsync(T aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            return aggregate.CommandsRouter.ExecuteAsync(aggregate, command, persistenceDelegate);
        }

        public IReadOnlyCollection<Type> RegisteredCommands { get; }
        public Type AggregateType { get; } = typeof(T);
    }

    public class MissingRegisteredCommandsException : Exception { }
}