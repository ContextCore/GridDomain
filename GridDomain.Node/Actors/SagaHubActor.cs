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

        public SagaHubActor(IPublisher publisher, 
                            IPersistentChildsRecycleConfiguration recycleConf, 
                            ISagaProducer<TSaga> sagaProducer ) : base(recycleConf, typeof(TSaga).Name)
        {
            _sagaStartMessages = new HashSet<Type>(sagaProducer.KnownDataTypes.Where(t => typeof(DomainEvent).IsAssignableFrom(t)));
            _publisher = publisher;
        }

        protected override string GetChildActorName(object message)
        {
            if (message is DomainEvent)
            {
                return AggregateActorName.New<TSagaState>(GetChildActorId(message)).ToString();
            }
            return null;
        }

        protected override Guid GetChildActorId(object message)
        {
            Guid childActorId = Guid.Empty;
            message.Match()
                   .With<DomainEvent>(m => childActorId = m.SagaId)
                   .With<ICommandFault>(m => childActorId = m.SagaId);
            return childActorId;
        }

        protected override void OnReceive(object message)
        {
            var msgType = message.GetType();
            DomainEvent domainEvent = message as DomainEvent;
            if (domainEvent != null && _sagaStartMessages.Contains(msgType) && domainEvent.SagaId == Guid.Empty)
            {
                //send message back to publisher to reroute to some hub according to SagaId
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