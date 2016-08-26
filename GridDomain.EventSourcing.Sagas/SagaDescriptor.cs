using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaDescriptor : ISagaDescriptor
    {
        private readonly List<Type> _acceptedMessages = new List<Type>();
        private readonly List<Type> _producedMessages = new List<Type>();
        private readonly List<Type> _startMessages = new List<Type>();
        public IReadOnlyCollection<Type> AcceptMessages => _acceptedMessages;
        public IReadOnlyCollection<Type> ProduceCommands => _producedMessages;
        public IReadOnlyCollection<Type> StartMessages => _startMessages;

        public Type StateType { get; } 
        public Type SagaType { get; }

        public SagaDescriptor(Type saga, Type state)
        {
            StateType = state;
            SagaType = saga;
        }

        public void AddAcceptedMessage(Type messageType)
        {
            _acceptedMessages.Add(messageType);
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