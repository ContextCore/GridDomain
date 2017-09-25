using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing {
    public class CommandAggregate : Aggregate, IAggregateCommandsHandlerDescriptor
    {
        protected CommandAggregate(Guid id) : base(id)
        {
            AggregateType = GetType();
        }

        protected CommandAggregate(IRouteEvents handler) : base(handler) { }
        //TODO: replace with types cache to reduce memory and increase performace
        internal readonly AggregateCommandsHandler<CommandAggregate> CommandsHandler = new AggregateCommandsHandler<CommandAggregate>();

        protected void Command<T>(Action<T> syncCommandAction) where T : ICommand
        {
            CommandsHandler.Map<T>(((c, a) => syncCommandAction(c)));
        }
        protected void Command<T>(Func<T,Task> asyncCommandAction) where T : ICommand
        {
            CommandsHandler.Map<T>(((c, a) => asyncCommandAction(c)));

        }
        protected void Command<T>(Func<T, CommandAggregate> aggregateCreator) where T : ICommand
        {
            CommandsHandler.Map<T>(aggregateCreator);
        }

        public Task ExecuteAsync(CommandAggregate aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            return CommandsHandler.ExecuteAsync(aggregate, command, persistenceDelegate);
        }

        public IReadOnlyCollection<Type> RegisteredCommands => CommandsHandler.RegisteredCommands;
        public Type AggregateType { get; }
    }
}