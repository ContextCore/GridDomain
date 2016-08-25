using System;
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
        private readonly ISagaDescriptor<TSaga> _sagaDescriptor;

        public SagaHubActor(IPublisher publisher, IPersistentChildsRecycleConfiguration recycleConf, ISagaDescriptor<TSaga> sagaDescriptor ) : base(recycleConf, typeof(TSaga).Name)
        {
            _sagaDescriptor = sagaDescriptor;
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

            if (domainEvent != null && _sagaDescriptor.StartMessages.Contains(msgType))
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