using System;
using CommonDomain.Core;

namespace GridDomain.Node.AkkaMessaging
{
    public class CreateActorRoute
    {
        public Type MessageType { get; }
        public Type ActorType { get; }

        public Type AggregateType { get; }
        
        public CreateActorRoute(Type messageType, Type actorType, Type aggregateType)
        {
            MessageType = messageType;
            ActorType = actorType;
            AggregateType = aggregateType;
        }

        public static CreateActorRoute New<TMessage, TAggregate, TAggregateActor>() where TAggregate: AggregateBase
                                                                                    where TAggregateActor: AggregateActor<TAggregate>
        {
            return new CreateActorRoute(typeof(TMessage), typeof(TAggregate), typeof(TAggregateActor));
        }
    }
}