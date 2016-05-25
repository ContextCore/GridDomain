using System;
using System.Collections.Generic;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsExecutor<T> where T : AggregateBase
    {
        private readonly Func<T> _aggregateProducer;

        public AggregateCommandsExecutor(Func<T> aggregateProducer)
        {
            _aggregateProducer = aggregateProducer;
        }

        public Guid GetAggregateId(ICommand command)
        {
            return Handler(command).GetId(command);
        }

        private AggregateCommandHandler<T> Handler(ICommand cmd)
        {
            return _commandHandlers[cmd.GetType()];
        }

        public IReadOnlyCollection<DomainEvent> Execute(ICommand command)
        {
            return Handler(command).Execute(_aggregateProducer.Invoke(), command);
        }

        private void Map(AggregateCommandHandler<T> handler)
        {
            _commandHandlers[typeof (T)] = handler;
        }

        protected void Map<TCommand>(Func<TCommand, Guid> idLocator, Action<TCommand, T> commandExecutor) where TCommand : ICommand
        {
            Map(AggregateCommandHandler<T>.New<TCommand>(idLocator, commandExecutor));
        }

        protected void Map<TCommand>(Func<TCommand, Guid> idLocator, Func<TCommand, T> commandExecutor) where TCommand : ICommand
        {
            Map(AggregateCommandHandler<T>.New<TCommand>(idLocator, commandExecutor));
        }

        private readonly IDictionary<Type, AggregateCommandHandler<T>> _commandHandlers = new Dictionary<Type, AggregateCommandHandler<T>>();
    }
}