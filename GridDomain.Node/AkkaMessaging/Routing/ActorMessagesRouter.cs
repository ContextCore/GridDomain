using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    public class ActorMessagesRouter : IMessagesRouter
    {
        private readonly IAggregateActorLocator _actorLocator;
        private readonly TypedMessageActor<CreateActorRoute> _routingActorTypedMessageActor;
        private readonly TypedMessageActor<CreateHandlerRoute> _routingTypedMessageActor;

        public ActorMessagesRouter(IActorRef routingActor, IAggregateActorLocator actorLocator)
        {
            _actorLocator = actorLocator;
            _routingTypedMessageActor = new TypedMessageActor<CreateHandlerRoute>(routingActor);
            _routingActorTypedMessageActor = new TypedMessageActor<CreateActorRoute>(routingActor);
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
            return new AkkaRouteBuilder<TMessage>(_routingTypedMessageActor, _routingActorTypedMessageActor,
                _actorLocator);
        }

        public void RegisterAggregate<TAggregate, TCommandHandler>()
            where TAggregate : AggregateBase
            where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
        {
            var messageRoutes = new TCommandHandler().GetRegisteredCommands().Select(c => new MessageRoute
            {
                CorrelationField = c.Property,
                MessageType = c.Command
            }).ToArray();

            var createActorRoute = new CreateActorRoute(typeof (TAggregate), messageRoutes);
            _routingActorTypedMessageActor.Handle(createActorRoute);
        }

        /// <summary>
        ///     Subscribe saga for all messages it can handle.
        ///     Messages are determined by implemented IHandler<T> interfaces
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        public void RegisterSaga<TSaga>() where TSaga : IDomainSaga
        {
            var allInterfaces = typeof (TSaga).GetInterfaces();
            var handlerInterfaces =
                allInterfaces.Where(i => i.IsGenericType &&
                                         i.GetGenericTypeDefinition() == typeof (IHandler<>))
                    .ToArray();
            var supportedMessages = handlerInterfaces.SelectMany(s => s.GetGenericArguments());
        }

        //TODO:replace with wait until event notifications
        public void WaitForRouteConfiguration()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}