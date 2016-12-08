using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Routing;

namespace GridDomain.Node
{

    public class ActorMessagesRouter : IMessagesRouter
    {
        private readonly IActorRef _routingActor;

        public ActorMessagesRouter(IActorRef routingActor)
        {
            _routingActor = routingActor;
        }

        public IRouteBuilder<TMessage> Route<TMessage>()
        {
            return new AkkaRouteBuilder<TMessage>(_routingActor);
        }

        public Task RegisterAggregate<TAggregate, TCommandHandler>()
            where TAggregate : AggregateBase
            where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
        {
            var descriptor = new AggregateCommandsHandlerDesriptor<TAggregate>();
            foreach(var info in new TCommandHandler().RegisteredCommands)
                descriptor.RegisterCommand(info.Command,info.Property);

            return RegisterAggregate(descriptor);
        }

        public Task RegisterAggregate(IAggregateCommandsHandlerDesriptor descriptor)
        {
            var messageRoutes = descriptor.RegisteredCommands.Select(c => new MessageRoute
             (
                 c.Command,
                 c.Property
             )).ToArray();

            var name = $"Aggregate_{descriptor.AggregateType.Name}";
            var createActorRoute = CreateActorRouteMessage.ForAggregate(descriptor.AggregateType, name, messageRoutes);
            return _routingActor.Ask<RouteCreated>(createActorRoute);
        }

        public Task RegisterSaga(ISagaDescriptor sagaDescriptor, string name)
        {
            var createActorRoute = CreateActorRouteMessage.ForSaga(sagaDescriptor, name);
            return _routingActor.Ask<RouteCreated>(createActorRoute);
        }

        public Task RegisterHandler<TMessage, THandler>(string correlationPropertyName) where THandler : IHandler<TMessage>
        {
            return Route<TMessage>().ToHandler<THandler>().WithCorrelation(correlationPropertyName).Register();
        }

        public Task RegisterProjectionGroup<T>(T @group) where T : IProjectionGroup
        {
            var createActorRoute = new CreateActorRouteMessage(typeof(SynchronizationProcessingActor<T>),
                                                               typeof(T).Name,
                                                               PoolKind.ConsistentHash,
                                                               @group.AcceptMessages.ToArray());
            return _routingActor.Ask<RouteCreated>(createActorRoute);
        }

    }
}