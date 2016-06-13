using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class CreateActorRoute
    {
        public CreateActorRoute(Type actorType, string actorName, params MessageRoute[] routes)
        {
            CheckForUniqueRoutes(routes);
            Routes = routes;
            ActorType = actorType;
            ActorName = actorName;
        }

        private void CheckForUniqueRoutes(MessageRoute[] routes)
        {
           var dublicateRoutes = routes.GroupBy(r => r.MessageType)
                                       .Where(g => g.Count() > 1)
                                       .ToArray();
            if (dublicateRoutes.Any())
                throw new DublicateRoutesException(dublicateRoutes.Select(r => r.Key.FullName));
        }

        public static CreateActorRoute ForAggregate<TAggregate>(string name, params MessageRoute[] routes) where TAggregate : AggregateBase
        {
            return new CreateActorRoute(typeof(AggregateHubActor<TAggregate>), name, routes);
        }

        public static CreateActorRoute ForSaga<TSaga, TSagaState, TStartMessage>(string name, params MessageRoute[] routes) 
            where TSaga : IDomainSaga 
            where TSagaState : AggregateBase 
            where TStartMessage : DomainEvent
        {
            return new CreateActorRoute(typeof(SagaHubActor<TSaga, TSagaState, TStartMessage>), name, routes);
        }

        public static CreateActorRoute ForSaga(ISagaDescriptor descriptor, string name)
        {
            var messageRoutes = descriptor.AcceptMessages
                .Select(eventType => new MessageRoute(eventType, nameof(DomainEvent.SagaId)))
                .ToArray();

            var actorOpenType = typeof(SagaHubActor<,,>);

            var actorType = actorOpenType.MakeGenericType(descriptor.SagaType, 
                                                          descriptor.StateType,
                                                          descriptor.StartMessage);

            return new CreateActorRoute(actorType, name, messageRoutes);
        }

        public MessageRoute[] Routes { get; }

        public Type ActorType { get; }

        public string ActorName { get; }
    }

    internal class DublicateRoutesException : Exception
    {
        public DublicateRoutesException(IEnumerable<string> dublicateRoutes)
            :base($"Found dublicate routes for messages: {string.Join(";",dublicateRoutes)}")
        {
        }
    }
}