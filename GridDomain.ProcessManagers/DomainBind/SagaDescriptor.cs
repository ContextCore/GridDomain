using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Automatonymous;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.ProcessManagers.DomainBind
{

    public class ProcessDescriptor : IProcessDescriptor
    {
        private readonly List<MessageBind> _acceptedMessages = new List<MessageBind>();
        private readonly List<Type> _producedMessages = new List<Type>();
        private readonly List<Type> _startMessages = new List<Type>();

        public ProcessDescriptor(Type state, Type stateMachine)
        {
            StateType = state;
            ProcessType = stateMachine;
        }

        public IReadOnlyCollection<MessageBind> AcceptMessages => _acceptedMessages;
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
            AddAcceptedMessage(typeof(TMessage));
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

        //type must be child of ProcessManager<TState>
        public static IProcessDescriptor ScanByConvention(Type processManagerType)
        {
            var stateType = processManagerType.GenericTypeArguments.First(t => typeof(IProcessState).IsAssignableFrom(t));
            var descriptor = new ProcessDescriptor(stateType, processManagerType);

            var domainBindedEvents = processManagerType.GetProperties()
                                                       .Where(p => p.PropertyType.IsConstructedGenericType
                                                                   && (p.PropertyType.GetGenericTypeDefinition() == typeof(Event<>) ||
                                                                       p.PropertyType.GetGenericTypeDefinition() == typeof(StartEvent<>)));
            foreach (var prop in domainBindedEvents)
            {
                var domainEventType = prop.PropertyType.GetGenericArguments().First();
                descriptor.AddAcceptedMessage(domainEventType);
                if(prop.PropertyType.GetGenericTypeDefinition() == typeof(StartEvent<>))
                    descriptor.AddStartMessage(domainEventType);
            }

            return descriptor;
        }

        public static ProcessDescriptor<TProcess, TState> CreateDescriptor<TProcess, TState>()
            where TState : class, IProcessState
            where TProcess : Process<TState>
        {
            var descriptor = new ProcessDescriptor<TProcess, TState>();

            var domainBindedEvents =
                typeof(TProcess).GetProperties()
                                .Where(p => p.PropertyType.IsConstructedGenericType
                                            && p.PropertyType.GetGenericTypeDefinition() == typeof(Event<>));
            foreach (var prop in domainBindedEvents)
            {
                var domainEventType = prop.PropertyType.GetGenericArguments()
                                          .First();
                descriptor.AddAcceptedMessage(domainEventType);
            }

            return descriptor;
        }
    }
}