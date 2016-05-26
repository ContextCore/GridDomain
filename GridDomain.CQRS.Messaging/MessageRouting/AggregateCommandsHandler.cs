using System;
using System.CodeDom;
using System.Collections.Generic;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsHandler<TAggregate>: IAggregateCommandsHandler<TAggregate>,
                                              ICommandAggregateLocator<TAggregate>
        where TAggregate : AggregateBase
    {
        public Guid GetAggregateId(ICommand command)
        {
            return GetHandler(command).GetId(command);
        }

        private AggregateCommandHandler<TAggregate> GetHandler(ICommand cmd)
        {
            AggregateCommandHandler<TAggregate> aggregateCommandHandler;//
            var commandType = cmd.GetType();
            if (!_commandHandlers.TryGetValue(commandType, out aggregateCommandHandler))
                throw new CannotFindAggregateCommandHandlerExeption(typeof (TAggregate), commandType);

            return aggregateCommandHandler;
        }

        private void Map<TCommand>(AggregateCommandHandler<TAggregate> handler)
        {
            _commandHandlers[typeof(TCommand)] = handler;
        }

        protected void Map<TCommand>(Func<TCommand, Guid> idLocator, Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map<TCommand>(AggregateCommandHandler<TAggregate>.New<TCommand>(idLocator, commandExecutor));
        }

        protected void Map<TCommand>(Func<TCommand, Guid> idLocator, Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map<TCommand>(AggregateCommandHandler<TAggregate>.New<TCommand>(idLocator, commandExecutor));
        }

        private readonly IDictionary<Type, AggregateCommandHandler<TAggregate>> _commandHandlers = new Dictionary<Type, AggregateCommandHandler<TAggregate>>();
        public IReadOnlyCollection<DomainEvent> Execute(TAggregate aggregate, ICommand command)
        {
            return GetHandler(command).Execute(aggregate, command);
        }
    }

    internal class CannotFindAggregateCommandHandlerExeption : Exception
    {
        public Type Type { get; }
        public Type CommandType { get; }

        public CannotFindAggregateCommandHandlerExeption(Type type, Type commandType)
        {
            Type = type;
            CommandType = commandType;
        }
    }
}