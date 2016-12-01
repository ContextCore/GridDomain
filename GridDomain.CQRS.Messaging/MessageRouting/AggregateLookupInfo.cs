using System;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateLookupInfo
    {
        public Type CommandType { get; }
        public string Property { get; }

        public AggregateLookupInfo(Type command, string property)
        {
            CommandType = command;
            Property = property;
        }
    }
}