using System;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandInfo
    {
        public AggregateCommandInfo(Type command)
        {
            CommandType = command;
        }

        public Type CommandType { get; }
        public string AggregateIdPropertyName { get; } = nameof(ICommand.AggregateId);
    }
}