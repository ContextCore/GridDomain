using System;

namespace GridDomain.EventSourcing.Sagas
{
    public class MessageBind
    {
        public MessageBind(Type messageType, string correlationField = nameof(DomainEvent.ProcessId))
        {
            MessageType = messageType;
            CorrelationField = correlationField;
        }

        public Type MessageType { get; }
        public string CorrelationField { get; }
    }
}