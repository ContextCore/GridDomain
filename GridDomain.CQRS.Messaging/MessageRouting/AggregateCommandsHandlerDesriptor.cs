using System;
using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
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
}