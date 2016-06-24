using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CommonDomain.Core;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace GridDomain.CQRS.Messaging.MessageRouting
{

    public interface  IAggregateCommandsHandlerDesriptor
    {
        IReadOnlyCollection<AggregateLookupInfo> RegisteredCommands { get; } 
        Type AggregateType { get; }
    }


    public class AggregateLookupInfo
    {
        public Type Command { get; }
        public string Property { get; }

        public AggregateLookupInfo(Type command, string property)
        {
            Command = command;
            Property = property;
        }
    }

    public class AggregateCommandsHandlerDesriptor<T> : IAggregateCommandsHandlerDesriptor
    {
  
        public void RegisterCommand<TCommand>(string property)
        {
            _registrations.Add(new AggregateLookupInfo(typeof(TCommand), property));
        }

        public void RegisterCommand(Type type,string property)
        {
            _registrations.Add(new AggregateLookupInfo(type, property));
        }

        private readonly List<AggregateLookupInfo> _registrations = new List<AggregateLookupInfo>();
        public IReadOnlyCollection<AggregateLookupInfo> RegisteredCommands => _registrations;
        public Type AggregateType => typeof(T);
    }

    public class AggregateCommandsHandler<TAggregate> : IAggregateCommandsHandler<TAggregate>,
                                                        ICommandAggregateLocator<TAggregate>
                                                        where TAggregate : AggregateBase
    {
        private readonly IDictionary<Type, AggregateCommandHandler<TAggregate>> _commandHandlers =
                                                     new Dictionary<Type, AggregateCommandHandler<TAggregate>>();

        private readonly IServiceLocator _serviceLocator;

        public AggregateCommandsHandler(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

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
            Map<TCommand>(AggregateCommandHandler<TAggregate>.New(idLocator, commandExecutor, _serviceLocator));
        }

        protected void Map<TCommand>(Expression<Func<TCommand, Guid>> idLocator,
            Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map<TCommand>(AggregateCommandHandler<TAggregate>.New(idLocator, commandExecutor, _serviceLocator));
        }

        public IReadOnlyCollection<AggregateLookupInfo> GetRegisteredCommands()
        {
            return _commandHandlers.Select(h => new AggregateLookupInfo (h.Key,h.Value.MachingProperty)).ToArray();
        }

    }
}