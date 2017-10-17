using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Automatonymous;
using GridDomain.EventSourcing;

namespace GridDomain.ProcessManagers.DomainBind
{

    public class ProcessDescriptor : IProcessDescriptor
    {
        private readonly List<MessageBind> _acceptedMessages = new List<MessageBind>();

        public ProcessDescriptor(Type state, Type stateMachine)
        {
            StateType = state;
            ProcessType = stateMachine;
        }

        public IReadOnlyCollection<MessageBind> AcceptMessages => _acceptedMessages;

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

        //type must be child of ProcessManager<TState>
        public static IProcessDescriptor ScanByConvention(Type processManagerType)
        {
            var stateType = ExtractStateType(processManagerType);
            var descriptor = new ProcessDescriptor(stateType, processManagerType);

            var domainBindedEvents = processManagerType.GetProperties()
                                                       .Where(p => p.PropertyType.IsConstructedGenericType
                                                                   && p.PropertyType.GetGenericTypeDefinition() == typeof(Event<>));
            foreach (var prop in domainBindedEvents)
            {
                var domainEventType = prop.PropertyType.GetGenericArguments().First();
                descriptor.AddAcceptedMessage(domainEventType);
            }

            return descriptor;
        }

        private static Type ExtractStateType(Type processManagerType)
        {
            var stateType = processManagerType.GetInterfaces()
                                     .FirstOrDefault(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IProcess<>))?
                                     .GenericTypeArguments?.FirstOrDefault();
            return stateType ?? throw new ArgumentException($"need type inherited from {typeof(IProcess<>)} to extract process state type");

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