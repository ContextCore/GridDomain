using System;
using System.Collections.Generic;
using CommonDomain;

namespace GridDomain.EventSourcing.Sagas
{

    public class MessageBinder
    {
        public MessageBinder(Type messageType, string correlationField = nameof(DomainEvent.SagaId))
        {
            MessageType = messageType;
            CorrelationField = correlationField;
        }

        public Type MessageType { get; }
        public string CorrelationField { get; }
    }

    public interface ISagaDescriptor
    {
        //TODO: enforce check all messages are DomainEvents
        IReadOnlyCollection<MessageBinder> AcceptMessages { get; }
        IReadOnlyCollection<Type> ProduceCommands { get; }
        IReadOnlyCollection<Type> StartMessages { get; } 
        Type StateType { get; }
        Type SagaType { get; }
    }
}