using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    public class SagaHubActor<TSaga, TSagaState> :
        PersistentHubActor where TSaga : class, ISagaInstance
        where TSagaState  : AggregateBase 
    {
        private readonly Type _actorType = typeof(SagaActor<TSaga, TSagaState>);
        private readonly IPublisher _publisher;
        private readonly HashSet<Type> _sagaStartMessages;
        private readonly Dictionary<Type, string> _acceptMessagesSagaIds;

        public SagaHubActor(IPublisher publisher, 
                            IPersistentChildsRecycleConfiguration recycleConf, 
                            ISagaProducer<TSaga> sagaProducer) : base(recycleConf, typeof(TSaga).Name)
        {
            _acceptMessagesSagaIds = sagaProducer.Descriptor.AcceptMessages.ToDictionary(m => m.MessageType, m=> m.CorrelationField);

            _sagaStartMessages = new HashSet<Type>(sagaProducer.KnownDataTypes.Where(t => typeof(DomainEvent).IsAssignableFrom(t)));
            _publisher = publisher;
        }

        protected override string GetChildActorName(object message)
        {
            return AggregateActorName.New<TSagaState>(GetChildActorId(message)).ToString();
        }

        protected override Guid GetChildActorId(object message)
        {
            Guid childActorId = Guid.Empty;

            message.Match().With<IMessageFault>(m => childActorId = m.SagaId);

            if(childActorId != Guid.Empty) return childActorId;

            string fieldName;
            var type = message.GetType();
            if (_acceptMessagesSagaIds.TryGetValue(type, out fieldName))
                childActorId = (Guid) type.GetProperty(fieldName).GetValue(message);

            return childActorId;
        }

        protected override void OnReceive(object message)
        {
            var msgType = message.GetType();

            DomainEvent domainEvent = message as DomainEvent;
            string routeField;
            _acceptMessagesSagaIds.TryGetValue(msgType, out routeField);

            if (domainEvent != null &&
                routeField == nameof(DomainEvent.SagaId) &&
                domainEvent.SagaId == Guid.Empty &&
                _sagaStartMessages.Contains(msgType))
            {
                //send message back to publisher to reroute to some hub according to SagaId
                //if message has custom mapping, no action is required
                _publisher.Publish(domainEvent.CloneWithSaga(Guid.NewGuid()));
                return;
            }
            
            base.OnReceive(message);
        }

        protected override Type GetChildActorType(object message)
        {
            return _actorType;
        }
    }
}