using System;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    internal class CannotFindAggregateCommandHandlerExeption : Exception
    {
        public CannotFindAggregateCommandHandlerExeption(Type type, Type commandType)
        {
            Type = type;
            CommandType = commandType;
        }

        public Type Type { get; }
        public Type CommandType { get; }
    }
}