using System;

namespace GridDomain.EventSourcing
{
    internal class CannotFindAggregateCommandHandlerExeption : Exception
    {
        public CannotFindAggregateCommandHandlerExeption()
        {
            
        }
        public CannotFindAggregateCommandHandlerExeption(Type type, Type commandType)
        {
            Type = type;
            CommandType = commandType;
        }

        public Type Type { get; }
        public Type CommandType { get; }
    }
}