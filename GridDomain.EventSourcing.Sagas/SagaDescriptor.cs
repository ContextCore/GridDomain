using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaDescriptor : ISagaDescriptor
    {
        private readonly List<MessageBinder> _acceptedMessages = new List<MessageBinder>();
        private readonly List<Type> _producedMessages = new List<Type>();
        private readonly List<Type> _startMessages = new List<Type>();
        public IReadOnlyCollection<MessageBinder> AcceptMessages => _acceptedMessages;
        public IReadOnlyCollection<Type> ProduceCommands => _producedMessages;
        public IReadOnlyCollection<Type> StartMessages => _startMessages;

        public Type StateType { get; } 
        public Type SagaType { get; }

        public SagaDescriptor(Type saga, Type state)
        {
            StateType = state;
            SagaType = saga;
        }

        public void AddAcceptedMessage(Type messageType, string correlationFieldName = nameof(DomainEvent.SagaId))
        {
            _acceptedMessages.Add(new MessageBinder(messageType,correlationFieldName));
        }

        public void AddProduceCommandMessage(Type messageType)
        {
            _producedMessages.Add(messageType);
        }

        public void AddStartMessage(Type messageType)
        {
            _startMessages.Add(messageType);
        }
    }
}