using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaDescriptor : ISagaDescriptor
    {

        public void AddAcceptedMessage(Type messageType)
        {
            _acceptedMessages.Add(messageType);
        }
        public void AddProduceCommandMessage(Type messageType)
        {
            _producedMessages.Add(messageType);
        }

        private List<Type> _acceptedMessages = new List<Type>(); 
        private  List<Type> _producedMessages = new List<Type>();

        public IReadOnlyCollection<Type> AcceptMessages => _acceptedMessages;
        public IReadOnlyCollection<Type> ProduceCommands => _producedMessages;
        public Type StartMessage { get; set; }
        public Type StateType { get; set; }
        public Type SagaType { get; set; }
    }
}