using System;
using Akka.Actor;
using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Persistence;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    class UnityServiceLocator : IServiceLocator
    {
        private readonly IUnityContainer _container;

        public UnityServiceLocator(IUnityContainer container)
        {
            _container = container;
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }


    //TODO: refactor to good config

    public static class CompositionRoot
    {
        public static void Init(IUnityContainer container,
            ActorSystem actorSystem,
            IDbConfiguration conf,
            TransportMode transportMode)
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

            //TODO: remove
            RegisterEventStore(container, conf);
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            container.RegisterType<IAggregateActorLocator, DefaultAggregateActorLocator>();
            container.RegisterType<ActorSystem>(new InjectionFactory(x => actorSystem));
            container.RegisterType<IServiceLocator,UnityServiceLocator>();
            Scheduling.CompositionRoot.Compose(container);
        }

        public static void RegisterEventStore(IUnityContainer container, IDbConfiguration conf)
        {
            container.RegisterInstance(conf);
            container.RegisterType<IConstructAggregates, AggregateFactory>();
        }
    }
}