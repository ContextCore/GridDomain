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
            _registeredRoutes = _eventsRouter = new ConventionEventRouter(GetType());
        }

        protected void Apply<T>(Action<T> act) where T : DomainEvent
        {
            _eventsRouter.Add<T>((a,e) => act((T)e));
        }

        private readonly ConventionEventRouter _eventsRouter;
        internal readonly AggregateCommandsHandler<CommandAggregate> CommandsRouter = new AggregateCommandsHandler<CommandAggregate>();

        protected void Execute<T>(Action<T> syncCommandAction) where T : ICommand
        {
            CommandsRouter.Map<T>(((c, a) => syncCommandAction(c)));
        }
        protected void Execute<T>(Func<T,Task> asyncCommandAction) where T : ICommand
        {
            CommandsRouter.Map<T>(((c, a) => asyncCommandAction(c)));
        }
        protected void Execute<T>(Func<T, CommandAggregate> aggregateCreator) where T : ICommand
        {
            CommandsRouter.Map<T>(aggregateCreator);
        }

        public IReadOnlyCollection<Type> RegisteredCommands => CommandsRouter.RegisteredCommands;
        public Type AggregateType { get; }
    }
}