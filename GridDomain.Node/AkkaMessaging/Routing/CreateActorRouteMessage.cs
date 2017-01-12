using System;
using System.Linq;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class CreateActorRouteMessage
    {

        public MessageRoute[] Routes { get; }

        public Type ActorType { get; }

        public string ActorName { get; }

        public PoolKind PoolKind { get; }

        public CreateActorRouteMessage(Type actorType, string actorName, PoolKind poolType, params MessageRoute[] routes)
        {
            CheckForUniqueRoutes(routes);
            Routes = routes;
            ActorType = actorType;
            ActorName = actorName;
            PoolKind = poolType;
        }

        private void CheckForUniqueRoutes(MessageRoute[] routes)
        {
           var dublicateRoutes = routes.GroupBy(r => r.MessageType)
                                       .Where(g => g.Count() > 1)
                                       .ToArray();
            if (dublicateRoutes.Any())
                throw new DublicateRoutesException(dublicateRoutes.Select(r => r.Key.FullName));
        }


        public static CreateActorRouteMessage ForAggregate(string name, IAggregateCommandsHandlerDesriptor descriptor)
        {
            var messageRoutes = descriptor.RegisteredCommands.Select(c => new MessageRoute
                (
                     MessageMetadataEnvelop.GenericForType(c.CommandType),
                     c.Property
                )).ToArray();

            var hubType = typeof(AggregateHubActor<>).MakeGenericType(descriptor.AggregateType);
            return new CreateActorRouteMessage(hubType, name, PoolKind.None, messageRoutes);
        }

        public static CreateActorRouteMessage ForSaga(ISagaDescriptor descriptor, string name = null)
        {
            name = name ??  $"SagaHub_{descriptor.SagaType.BeautyName()}";

            var messageRoutes = descriptor.AcceptMessages
                .Select(messageBinder => new MessageRoute(
                        MessageMetadataEnvelop.GenericForType(messageBinder.MessageType), 
                        messageBinder.CorrelationField))
                .ToArray();

            var hubType = typeof(SagaHubActor<,>).MakeGenericType(descriptor.SagaType, 
                                                                  descriptor.StateType);

            return new CreateActorRouteMessage(hubType, name, PoolKind.None, messageRoutes);
        }
    }
}