using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging.MessageRouting;
using NLog;

namespace GridDomain.Node.AkkaMessaging
{

    public class ActorMessagesRouter : IMessagesRouter
    {
        private readonly TypedMessageActor<CreateHandlerRoute> _routingTypedMessageActor;
        private readonly TypedMessageActor<CreateActorRoute> _routingActorTypedMessageActor;
        private readonly IAggregateActorLocator _actorLocator;

        public ActorMessagesRouter(IActorRef routingActor, IAggregateActorLocator actorLocator)
        {
            _actorLocator = actorLocator;
            _routingTypedMessageActor = new TypedMessageActor<CreateHandlerRoute>(routingActor);
            _routingActorTypedMessageActor = new TypedMessageActor<CreateActorRoute>(routingActor);
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
            return new AkkaRouteBuilder<TMessage>(_routingTypedMessageActor, _routingActorTypedMessageActor, _actorLocator);
        }

        public void Register<TAggregate, TCommandHandler>(TCommandHandler handler) where TAggregate : AggregateBase where TCommandHandler : AggregateCommandsHandler<TAggregate>
        {
            var messageRoutes = handler.GetRegisteredCommands().Select(c => new MessageRoute()
            {
                CorrelationField = c.Property,
                MessageType = c.Command
            }).ToArray();

            var createActorRoute = new CreateActorRoute(typeof(TAggregate), messageRoutes);
            _routingActorTypedMessageActor.Handle(createActorRoute);
        }

        //TODO:replace with wait until event notifications
        public void WaitForRouteConfiguration()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}