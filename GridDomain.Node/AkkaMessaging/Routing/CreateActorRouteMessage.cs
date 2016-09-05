using System;
using System.Linq;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class CreateActorRouteMessage
    {
        public CreateActorRouteMessage(Type actorType, string actorName, params MessageRoute[] routes)
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

        public static CreateActorRouteMessage ForAggregate<TAggregate>(string name, params MessageRoute[] routes) where TAggregate : AggregateBase
        {
            return ForAggregate(typeof(TAggregate), name, routes);
        }
        public static CreateActorRouteMessage ForAggregate(Type aggregateType, string name, params MessageRoute[] routes)
        {
           return new CreateActorRouteMessage(typeof(AggregateHubActor<>).MakeGenericType(aggregateType), name, routes);
        }

        public static CreateActorRouteMessage ForSaga<TSaga, TSagaState>(string name, params MessageRoute[] routes) 
            where TSaga : class, ISagaInstance 
            where TSagaState : AggregateBase 
        {
            return new CreateActorRouteMessage(typeof(SagaHubActor<TSaga, TSagaState>), name, routes);
        }

        public static CreateActorRouteMessage ForSaga(ISagaDescriptor descriptor, string name = null)
        {
            name = name ??  $"SagaHub_{descriptor.SagaType.BeautyName()}";

            var messageRoutes = descriptor.AcceptMessages
                .Select(messageBinder => new MessageRoute(messageBinder.MessageType, messageBinder.CorrelationField))
                .ToArray();

            var actorType = typeof(SagaHubActor<,>).MakeGenericType(descriptor.SagaType, 
                                                                    descriptor.StateType);

            return new CreateActorRouteMessage(actorType, name, messageRoutes);
        }

        public MessageRoute[] Routes { get; }

        public Type ActorType { get; }

        public string ActorName { get; }
    }
}