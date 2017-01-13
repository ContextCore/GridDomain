using System;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandInfo
    {
        public Type CommandType { get; }
        public string AggregateIdPropertyName { get; }

        public AggregateCommandInfo(Type command, string aggregateIdPropertyName)
        {
            CommandType = command;
            AggregateIdPropertyName = aggregateIdPropertyName;
        }
    }
}