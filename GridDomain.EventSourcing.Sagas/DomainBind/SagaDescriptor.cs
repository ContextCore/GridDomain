using System;
using System.Collections.Generic;
using System.Linq;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Processes.DomainBind
{
    public class ProcessManagerDescriptor : IProcessManagerDescriptor
    {
        private readonly List<MessageBind> _acceptedMessages = new List<MessageBind>();
        private readonly List<Type> _producedMessages = new List<Type>();
        private readonly List<Type> _startMessages = new List<Type>();

        public ProcessManagerDescriptor(Type state, Type stateMachine)
        {
            StateType = state;
            ProcessType = stateMachine;
        }

        public IReadOnlyCollection<MessageBind> AcceptMessages => _acceptedMessages;
        public IReadOnlyCollection<Type> ProduceCommands => _producedMessages;
        public IReadOnlyCollection<Type> StartMessages => _startMessages;

        public Type StateType { get; }
        public Type ProcessType { get; }

        public void AddAcceptedMessage(Type messageType, string correlationFieldName = nameof(DomainEvent.ProcessId))
        {
            _acceptedMessages.RemoveAll(b => b.MessageType == messageType);
            _acceptedMessages.Add(new MessageBind(messageType, correlationFieldName));
        }

        public void AddAcceptedMessage<TMessage>() where TMessage : IHaveProcessId
        {
            AddAcceptedMessage(typeof(TMessage), nameof(IHaveProcessId.ProcessId));
        }

        public void AddProduceCommandMessage(Type messageType)
        {
            _producedMessages.RemoveAll(t => t == messageType);
            _producedMessages.Add(messageType);
        }

        public void AddCommand<T>() where T : ICommand
        {
            AddProduceCommandMessage(typeof(T));
        }

        public void AddStartMessage(Type messageType)
        {
            _startMessages.RemoveAll(t => t == messageType);
            _startMessages.Add(messageType);
        }

        public void AddStartMessage<T>()
        {
            AddStartMessage(typeof(T));
        }

        public static ProcessManagerDescriptor<TSaga, TSagaData> CreateDescriptor<TSaga, TSagaData>()
            where TSagaData : class, IProcessState where TSaga : Process<TSagaData>
        {
            var sagaDescriptor = new ProcessManagerDescriptor<TSaga, TSagaData>();

            var domainBindedEvents =
                typeof(TSaga).GetProperties()
                             .Where(
                                    p =>
                                        p.PropertyType.IsGenericType
                                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Event<>));
            foreach (var prop in domainBindedEvents)
            {
                var domainEventType = prop.PropertyType.GetGenericArguments().First();
                sagaDescriptor.AddAcceptedMessage(domainEventType);
            }

            return sagaDescriptor;
        }
    }
}