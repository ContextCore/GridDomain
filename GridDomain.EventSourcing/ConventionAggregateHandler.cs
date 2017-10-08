using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing {
    public class ConventionAggregateHandler<T> : IAggregateCommandsHandler<T> where T : CommandAggregate
    {

        public ConventionAggregateHandler(T aggregate)
        {
            RegisteredCommands = aggregate.RegisteredCommands;
            if (!RegisteredCommands.Any())
                throw new MissingRegisteredCommandsException();
        }

        public async Task<T> ExecuteAsync(T aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            return (T)await aggregate.ExecuteAsync(aggregate, command, persistenceDelegate);
        }

        public IReadOnlyCollection<Type> RegisteredCommands { get; }
        public Type AggregateType { get; } = typeof(T);
    }
}