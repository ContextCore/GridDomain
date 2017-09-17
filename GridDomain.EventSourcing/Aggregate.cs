using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{


    public class CommandAggregateHandlerProxy<T> : IAggregateCommandsHandler<T> where T : CommandAggregate
    {
        private readonly CommandAggregate _aggregate;

        public CommandAggregateHandlerProxy(CommandAggregate aggregate)
        {
            _aggregate = aggregate;
        }

        public Task ExecuteAsync(T aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            return _aggregate._commandsHandler.ExecuteAsync(aggregate, command, persistenceDelegate);
        }

        public IReadOnlyCollection<Type> RegisteredCommands => _aggregate.RegisteredCommands;
        public Type AggregateType => _aggregate.AggregateType;
    }

    public class CommandAggregate : Aggregate
    {
        protected CommandAggregate(Guid id) : base(id)
        {
            AggregateType = GetType();
        }

        protected CommandAggregate(IRouteEvents handler) : base(handler) { }
        //TODO: replace with types cache to reduce memory and increase performace
        internal readonly AggregateCommandsHandler<CommandAggregate> _commandsHandler = new AggregateCommandsHandler<CommandAggregate>();

        protected void Command<T>(Action<T> syncCommandAction) where T : ICommand
        {
            _commandsHandler.Map<T>(((c, a) => syncCommandAction(c)));
        }
        protected void Command<T>(Func<T,Task> asyncCommandAction) where T : ICommand
        {
            _commandsHandler.Map<T>(((c, a) => asyncCommandAction(c)));

        }
        protected void Command<T>(Func<T, CommandAggregate> aggregateCreator) where T : ICommand
        {
            _commandsHandler.Map<T>(aggregateCreator);
        }

        public Task ExecuteAsync(CommandAggregate aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            return _commandsHandler.ExecuteAsync(aggregate, command, persistenceDelegate);
        }

        public IReadOnlyCollection<Type> RegisteredCommands => _commandsHandler.RegisteredCommands;
        public Type AggregateType { get; }
    } 


    public class Aggregate : IAggregate,
                                      IMemento,
                                      IEquatable<IAggregate>
    {
        private static readonly AggregateFactory Factory = new AggregateFactory();
        public static T Empty<T>(Guid? id = null) where T : IAggregate
        {
            return Factory.Build<T>(id ?? Guid.NewGuid());
        }

        private readonly ICollection<object> _uncommittedEvents = new LinkedList<object>();
        public bool HasUncommitedEvents => _uncommittedEvents.Any();
        private IRouteEvents _registeredRoutes;

        private PersistenceDelegate _persist;

        public void SetPersistProvider(PersistenceDelegate caller)
        {
            _persist = caller;
        }

        protected Aggregate(Guid id) : this(null)
        {
            Id = id;
        }

        protected Aggregate(IRouteEvents handler)
        {
            if (handler == null)
                return;

            RegisteredRoutes = handler;
            RegisteredRoutes.Register(this);
        }

        Guid IMemento.Id
        {
            get => Id;
            set => Id = value;
        }

        int IMemento.Version
        {
            get => Version;
            set => Version = value;
        }

        protected void Apply<T>(Action<T> action) where T : DomainEvent
        {
            Register(action);
        }

        public virtual IMemento GetSnapshot()
        {
            return this;
        }

        protected IRouteEvents RegisteredRoutes
        {
            get => _registeredRoutes ?? (_registeredRoutes = new ConventionEventRouter(true, this));
            set => _registeredRoutes = value ?? throw new InvalidOperationException("AggregateBase must have an event router to function");
        }

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        void IAggregate.ApplyEvent(object @event)
        {
            RegisteredRoutes.Dispatch(@event);
            Version++;
        }

        ICollection IAggregate.GetUncommittedEvents()
        {
            return (ICollection) _uncommittedEvents;
        }

        void IAggregate.ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        public virtual bool Equals(IAggregate other)
        {
            return null != other && other.Id == Id;
        }

        protected void Register<T>(Action<T> route)
        {
            RegisteredRoutes.Register(route);
        }

        protected async Task Emit<T>(Task<T> evtTask) where T : DomainEvent
        {
            await Emit(await evtTask);
        }

        public bool MarkPersisted(DomainEvent e)
        {
            if (!_uncommittedEvents.Contains(e))
                throw new EventIsNotBelongingToAggregateException();

            ((IAggregate) this).ApplyEvent(e);
            return _uncommittedEvents.Remove(e);
        }

        protected async Task Emit(params DomainEvent[] events)
        {
            Produce(events);
            await _persist(this);
        }

        protected void Produce(params DomainEvent[] events)
        {
            foreach(var e in events)
            {
                _uncommittedEvents.Add(e);
            }
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IAggregate);
        }
    }
}