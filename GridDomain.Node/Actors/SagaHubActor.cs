using System;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.Node.Actors
{
    public class SagaHubActor<TSaga, TSagaState, TStartMessage> :
        PersistentHubActor where TSaga : IDomainSaga
        where TSagaState  : AggregateBase 
        where TStartMessage : DomainEvent
    {
        private readonly Type _actorType = typeof (SagaActor<TSaga, TSagaState, TStartMessage>);

        protected override string GetChildActorName(object message)
        {
            if (message is DomainEvent)
            {
                return $"Saga_{typeof(TSaga).Name}_{GetChildActorId(message)}";
            }
            return null;
        }

        protected override Guid GetChildActorId(object message)
        {
            if (message is TStartMessage)
            {
                return Guid.NewGuid();
            }

            if (message is DomainEvent)
            {
                return (message as DomainEvent).SagaId;
            }
            return Guid.Empty;
        }

        protected override Type GetChildActorType(object message)
        {
            return _actorType;
        }
    }
}