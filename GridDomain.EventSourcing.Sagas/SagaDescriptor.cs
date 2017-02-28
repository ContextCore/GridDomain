using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Automatonymous;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaDescriptor<TSaga, TSagaData> : SagaDescriptor where TSaga : Saga<TSagaData>
                                                                   where TSagaData : class, ISagaState

    {
        public SagaDescriptor()
            : base(typeof(ISagaInstance<TSaga, TSagaData>), typeof(SagaStateAggregate<TSagaData>), typeof(TSaga)) {}

        public void MapDomainEvent<TDomainEvent>(Expression<Func<TSaga, Event<TDomainEvent>>> machineEvent,
                                                 Expression<Func<TDomainEvent, Guid>> correlationFieldExpression)
        {
            AddAcceptedMessage(typeof(TDomainEvent), MemberNameExtractor.GetName(correlationFieldExpression));
        }
    }

    public class SagaDescriptor : ISagaDescriptor
    {
        private readonly List<MessageBind> _acceptedMessages = new List<MessageBind>();
        private readonly List<Type> _producedMessages = new List<Type>();
        private readonly List<Type> _startMessages = new List<Type>();

        public SagaDescriptor(Type saga, Type state, Type stateMachine)
        {
            StateType = state;
            SagaType = saga;
            StateMachineType = stateMachine;
        }

        public IReadOnlyCollection<MessageBind> AcceptMessages => _acceptedMessages;
        public IReadOnlyCollection<Type> ProduceCommands => _producedMessages;
        public IReadOnlyCollection<Type> StartMessages => _startMessages;

        public Type StateType { get; }
        public Type SagaType { get; }
        public Type StateMachineType { get; }

        public void AddAcceptedMessage(Type messageType, string correlationFieldName = nameof(DomainEvent.SagaId))
        {
            _acceptedMessages.RemoveAll(b => b.MessageType == messageType);
            _acceptedMessages.Add(new MessageBind(messageType, correlationFieldName));
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

        public static SagaDescriptor<TSaga, TSagaData> CreateDescriptor<TSaga, TSagaData>()
            where TSagaData : class, ISagaState where TSaga : Saga<TSagaData>
        {
            var sagaDescriptor = new SagaDescriptor<TSaga, TSagaData>();

            var domainBindedEvents = typeof(TSaga).GetProperties()
                                                  .Where(
                                                      p =>
                                                          p.PropertyType.IsGenericType
                                                          && p.PropertyType.GetGenericTypeDefinition() == typeof(Event<>));
            foreach (var prop in domainBindedEvents)
            {
                var domainEventType = prop.PropertyType.GetGenericArguments()
                                          .First();
                sagaDescriptor.AddAcceptedMessage(domainEventType);
            }

            return sagaDescriptor;
        }
    }
}