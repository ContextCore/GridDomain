using System;
using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsHandlerDescriptor<T> : IAggregateCommandsHandlerDescriptor
    {
        public void RegisterCommand<TCommand>()
        {
            RegisterCommand(typeof(TCommand));
        }

        public void RegisterCommand(Type type)
        {
            _registrations.Add(new AggregateCommandInfo(type));
        }

        private readonly List<AggregateCommandInfo> _registrations = new List<AggregateCommandInfo>();
        public IReadOnlyCollection<AggregateCommandInfo> RegisteredCommands => _registrations;
        public Type AggregateType => typeof(T);
    }
}