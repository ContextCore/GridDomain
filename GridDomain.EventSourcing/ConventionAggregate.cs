using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing 
{
    
    public class ConventionAggregate : Aggregate
    {
        protected ConventionAggregate(string id) : base(id)
        {
            _eventsRouter = new ConventionEventRouter(GetType());

        }

        protected override void OnApplyEvent(DomainEvent evt)
        {
            _eventsRouter.Dispatch(this, evt);
        }

        public override Task<IReadOnlyCollection<DomainEvent>> Execute(ICommand command)
        {
            var func = _commandsRouter.Get(command);
            if(func == null)
                throw new UnknownCommandExeption();
            _uncommittedEvents.Clear();
            return func.Invoke(command) ?? NoEvents;
        }

        private Task<IReadOnlyCollection<DomainEvent>> NoEvents { get; } = Task.FromResult<IReadOnlyCollection<DomainEvent>>(new DomainEvent[] { });
        private readonly ConventionEventRouter _eventsRouter;
        private readonly TypeCatalog<Func<ICommand,Task<IReadOnlyCollection<DomainEvent>>>,ICommand> _commandsRouter = 
            new TypeCatalog<Func<ICommand,Task<IReadOnlyCollection<DomainEvent>>>,ICommand>();

        protected void Execute<T>(Action<T> syncCommandAction) where T : ICommand
        {
            _commandsRouter.Add<T>(async c =>
                                   {
                                       syncCommandAction((T)c);
                                       return await Task.FromResult(_uncommittedEvents);
                                   });
        }
        
        protected void Execute<T>(Func<T, Task> asyncCommandAction) where T : ICommand
        {
            _commandsRouter.Add<T>( async c  =>
                                    {
                                        await asyncCommandAction((T) c);
                                        return await Task.FromResult(_uncommittedEvents);
                                    });
        }
        
        protected void Apply<T>(Action<T> act) where T : DomainEvent
        {
            _eventsRouter.Add<T>((a, e) => act((T)e));
        }

    }
}