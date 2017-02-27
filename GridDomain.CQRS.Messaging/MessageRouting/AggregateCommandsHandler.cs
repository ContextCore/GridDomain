using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain.Core;
using Microsoft.Practices.ServiceLocation;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsHandler<TAggregate> : IAggregateCommandsHandler<TAggregate>
                                                        where TAggregate : AggregateBase
    {
        private readonly IDictionary<Type, AggregateCommandHandler<TAggregate>> _commandHandlers =
                                                     new Dictionary<Type, AggregateCommandHandler<TAggregate>>();


        public TAggregate Execute(TAggregate aggregate, ICommand command)
        {
            return GetHandler(command).Execute(aggregate, command);
        }

        private AggregateCommandHandler<TAggregate> GetHandler(ICommand cmd)
        {
            AggregateCommandHandler<TAggregate> aggregateCommandHandler; 
            var commandType = cmd.GetType();

            if (!_commandHandlers.TryGetValue(commandType, out aggregateCommandHandler))
                throw new CannotFindAggregateCommandHandlerExeption(typeof (TAggregate), commandType);

            return aggregateCommandHandler;
        }

        private void Map<TCommand>(AggregateCommandHandler<TAggregate> handler)
        {
            _commandHandlers[typeof(TCommand)] = handler;
        }

        public void Map<TCommand>(Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map<TCommand>(AggregateCommandHandler<TAggregate>.New(commandExecutor));
        }

        protected void Map<TCommand>(Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map<TCommand>(AggregateCommandHandler<TAggregate>.New(commandExecutor));
        }

        public IReadOnlyCollection<AggregateCommandInfo> RegisteredCommands
        {
            get
            {
                return _commandHandlers.Select(h => new AggregateCommandInfo(h.Key)).ToArray();
            }
        }
    }
}