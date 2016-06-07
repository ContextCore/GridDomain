using System;
using CommonDomain.Core;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class CreateActorRoute
    {
        private CreateActorRoute(Type actorType, string actorName, params MessageRoute[] routes)
        {
            Routes = routes;
            ActorType = actorType;
            ActorName = actorName;
        }

        public static CreateActorRoute ForAggregate<TAggregate>(string name, params MessageRoute[] routes) where TAggregate : AggregateBase
        {
            return new CreateActorRoute(typeof(AggregateHostActor<TAggregate>), name, routes);
        }

        public static CreateActorRoute ForSaga<TSaga, TSagaState, TStartMessage>(string name, params MessageRoute[] routes) 
            where TSaga : IDomainSaga where TSagaState : AggregateBase
        {
            return new CreateActorRoute(typeof(SagaActor<TSaga, TSagaState, TStartMessage>), name, routes);
        }

        public MessageRoute[] Routes { get; }

        public Type ActorType { get; }

        public string ActorName { get; }
    }
}