using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
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
            (
                c.Command,
                c.Property
            )).ToArray();

            var name = $"Aggregate_{typeof(TAggregate).Name}";
            var createActorRoute = CreateActorRoute.ForAggregate<TAggregate>(name,messageRoutes);
            _routingActorTypedMessageActor.Handle(createActorRoute);
        }

        public void RegisterSaga(ISagaDescriptor sagaDescriptor)
        {
            var name = $"Saga_{sagaDescriptor.SagaType.Name}";
            var createActorRoute = CreateActorRoute.ForSaga(sagaDescriptor, name);
            _routingActorTypedMessageActor.Handle(createActorRoute);
        }

        /// <summary>
        ///     Subscribe saga for all messages it can handle.
        ///     Messages are determined by implemented IHandler<T> interfaces
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        public void RegisterSaga<TSaga, TSagaState, TStartMessage>()
                                         where TSaga : IDomainSaga
                                         where TSagaState : AggregateBase
                                         where TStartMessage : DomainEvent
        {
           
            var messageRoutes = SagaInfo<TSaga>.KnownMessages().Select(m => new MessageRoute(m, nameof(DomainEvent.SagaId))).ToArray();
            var name = $"Saga_{typeof(TSaga).Name}";
            var createActorRoute = CreateActorRoute.ForSaga<TSaga,TSagaState,TStartMessage>(name, messageRoutes);
            _routingActorTypedMessageActor.Handle(createActorRoute);
        }

        //TODO:replace with wait until event notifications
        public void WaitForRouteConfiguration()
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}