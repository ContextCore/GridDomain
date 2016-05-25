using System;
using System.Collections.Generic;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsHandler<T>: IAggregateCommandsHandler<T>,
                                              ICommandAggregateLocator<T>
        where T : AggregateBase
    {
        public Guid GetAggregateId(ICommand command)
        {
            return GetHandler(command).GetId(command);
        }

        private AggregateCommandHandler<T> GetHandler(ICommand cmd)
        {
            return _commandHandlers[cmd.GetType()];
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
        public IReadOnlyCollection<DomainEvent> Execute(T aggregate, ICommand command)
        {
            return GetHandler(command).Execute(aggregate, command);
        }
    }
}