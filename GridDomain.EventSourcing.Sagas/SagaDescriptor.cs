using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaDescriptor
    {
        private readonly List<Type> _acceptedMessages = new List<Type>();
        private readonly List<Type> _producedMessages = new List<Type>();
        private readonly List<Type> _startMessages = new List<Type>();
        public IReadOnlyCollection<Type> AcceptMessages => _acceptedMessages;
        public IReadOnlyCollection<Type> ProduceCommands => _producedMessages;
        public IReadOnlyCollection<Type> StartMessages => _startMessages;

        public Type StateType { get; } 
        public Type SagaType { get; }

        public SagaDescriptor(Type state, Type saga)
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
            _acceptedMessages.Add(messageType);
        }
    }

    public class SagaDescriptor<TSaga,TState> : SagaDescriptor, ISagaDescriptor<TSaga>
    {
        public SagaDescriptor():base(typeof(TSaga), typeof(TState))
        {
            
        }
    }
}