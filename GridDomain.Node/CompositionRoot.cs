using Akka.Actor;
using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Scheduling.Quartz;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    //TODO: refactor to good config via IContainerConfiguration
    public static class CompositionRoot
    {
    //TODO: refactor to good config via IContainerConfiguration
        public static void Init(IUnityContainer container,
                                ActorSystem actorSystem,
                                TransportMode transportMode,
                                IQuartzConfig config = null)
        {
            //TODO: replace with config

            if (transportMode == TransportMode.Standalone)
            {
                var transport = new AkkaEventBusTransport(actorSystem);
                container.RegisterInstance<IPublisher>(transport);
                container.RegisterInstance<IActorSubscriber>(transport);
            }
            if (transportMode == TransportMode.Cluster)
            {
                var transport = new DistributedPubSubTransport(actorSystem);
                container.RegisterInstance<IPublisher>(transport);
                container.RegisterInstance<IActorSubscriber>(transport);
            }
            
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            container.RegisterType<IAggregateActorLocator, DefaultAggregateActorLocator>();
            container.RegisterInstance(actorSystem);

            //TODO: replace with better implementation
            Scheduling.CompositionRoot.Compose(container, config ?? new PersistedQuartzConfig());
        }

    }

}