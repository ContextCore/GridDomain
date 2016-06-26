using System;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateLookupInfo
    {
        public Type Command { get; }
        public string Property { get; }

        public AggregateLookupInfo(Type command, string property)
        {
            Command = command;
            Property = property;
        }
    }
}