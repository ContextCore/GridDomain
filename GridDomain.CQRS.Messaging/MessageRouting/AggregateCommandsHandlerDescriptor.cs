using System;
using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsHandlerDescriptor<T> : IAggregateCommandsHandlerDescriptor
    {
  
        public void RegisterCommand<TCommand>(string property)
        {
            _registrations.Add(new AggregateCommandInfo(typeof(TCommand), property));
        }

        public void RegisterCommand(Type type,string property)
        {
            _registrations.Add(new AggregateCommandInfo(type, property));
        }

        private readonly List<AggregateCommandInfo> _registrations = new List<AggregateCommandInfo>();
        public IReadOnlyCollection<AggregateCommandInfo> RegisteredCommands => _registrations;
        public Type AggregateType => typeof(T);
    }
}