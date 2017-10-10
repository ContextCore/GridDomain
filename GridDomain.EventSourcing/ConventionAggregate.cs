using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing {
    public class ConventionAggregate : CommandAggregate
    {
        protected ConventionAggregate(Guid id) : base(id)
        {
            _eventsRouter = new ConventionEventRouter(GetType());

        }

        public override IReadOnlyCollection<Type> RegisteredCommands => _commandsRouter.RegisteredCommands;

        protected override async Task<IAggregate> Execute(ICommand cmd)
        {
            return await _commandsRouter.ExecuteAsync(this, cmd, EventStore);
        }
        
        protected override void OnAppyEvent(DomainEvent evt)
        {
            _eventsRouter.Dispatch(this, evt);
        }

        private readonly ConventionEventRouter _eventsRouter;
        private readonly AggregateCommandsHandler<ConventionAggregate> _commandsRouter = new AggregateCommandsHandler<ConventionAggregate>();

        protected void Execute<T>(Action<T> syncCommandAction) where T : ICommand
        {
            _commandsRouter.Map<T>(((c, a) => syncCommandAction(c)));
        }
        protected void Execute<T>(Func<T, Task> asyncCommandAction) where T : ICommand
        {
            _commandsRouter.Map<T>(((c, a) => asyncCommandAction(c)));
        }
        protected void Execute<T>(Func<T, ConventionAggregate> aggregateCreator) where T : ICommand
        {
            _commandsRouter.Map<T>(aggregateCreator);
        }

        protected void Apply<T>(Action<T> act) where T : DomainEvent
        {
            _eventsRouter.Add<T>((a, e) => act((T)e));
        }

    }
}