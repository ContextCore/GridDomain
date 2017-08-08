using System;

namespace GridDomain.EventSourcing.Aggregates
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