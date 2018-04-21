using System;
using GridDomain.EventSourcing;

namespace GridDomain.ProcessManagers.DomainBind
{
    public class MessageBind
    {
        public MessageBind(Type messageType, string correlationField = nameof(DomainEvent.ProcessId))
        {
            MessageType = messageType;
            CorrelationField = correlationField;
        }

        public Type MessageType { get; }
        //TODO:Remove not used field
        public string CorrelationField { get; }
    }
}