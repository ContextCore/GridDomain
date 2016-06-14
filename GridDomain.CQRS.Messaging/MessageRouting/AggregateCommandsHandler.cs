using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsHandler<TAggregate> : IAggregateCommandsHandler<TAggregate>,
        ICommandAggregateLocator<TAggregate>
        where TAggregate : AggregateBase
    {
        private readonly IDictionary<Type, AggregateCommandHandler<TAggregate>> _commandHandlers =
            new Dictionary<Type, AggregateCommandHandler<TAggregate>>();

        public TAggregate Execute(TAggregate aggregate, ICommand command)
        {
            return GetHandler(command).Execute(aggregate, command);
        }

        public Guid GetAggregateId(ICommand command)
        {
            return GetHandler(command).GetId(command);
        }

        private AggregateCommandHandler<TAggregate> GetHandler(ICommand cmd)
        {
            AggregateCommandHandler<TAggregate> aggregateCommandHandler; //
            var commandType = cmd.GetType();
            if (!_commandHandlers.TryGetValue(commandType, out aggregateCommandHandler))
                throw new CannotFindAggregateCommandHandlerExeption(typeof (TAggregate), commandType);

            return aggregateCommandHandler;
        }

        private void Map<TCommand>(AggregateCommandHandler<TAggregate> handler)
        {
            _commandHandlers[typeof (TCommand)] = handler;
        }

        protected void Map<TCommand>(Expression<Func<TCommand, Guid>> idLocator,
            Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map<TCommand>(AggregateCommandHandler<TAggregate>.New(idLocator, commandExecutor));
        }

        protected void Map<TCommand>(Expression<Func<TCommand, Guid>> idLocator,
            Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map<TCommand>(AggregateCommandHandler<TAggregate>.New(idLocator, commandExecutor));
        }

        public IReadOnlyCollection<AggregateLookupInfo> GetRegisteredCommands()
        {
            return
                _commandHandlers.Select(
                    h => new AggregateLookupInfo {Command = h.Key, Property = h.Value.MachingProperty})
                    .ToArray();
        }

        public class AggregateLookupInfo
        {
            public Type Command;
            public string Property;
        }
    }
}