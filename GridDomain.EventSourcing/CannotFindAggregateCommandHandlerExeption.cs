using System;

namespace GridDomain.EventSourcing
{
    public class UnknownCommandExeption : Exception
    {
        public UnknownCommandExeption()
        {
            
        }
        public UnknownCommandExeption(Type type, Type commandType)
        {
            Type = type;
            CommandType = commandType;
        }

        public Type Type { get; }
        public Type CommandType { get; }
    }
}