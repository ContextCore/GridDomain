using System;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class MessageProcessException : Exception
    {
        public Type Type { get; set; }

        public MessageProcessException(Type type, Exception error):base($"Error occured while processing message in {type}",error)
        {
            Type = type;
        }
    }
}