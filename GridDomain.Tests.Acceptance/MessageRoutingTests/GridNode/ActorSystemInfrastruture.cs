using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.TestKit.NUnit;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Routing;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem.Setup;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode
{
    public abstract class ActorSystemInfrastruture : IDisposable
    {
        public readonly AkkaConfiguration AkkaConfig;
        public ActorMessagesRouter Router;


        protected ActorSystemInfrastruture(AkkaConfiguration conf)
        {
            AkkaConfig = conf;
        }

        protected abstract IActorSubscriber Subscriber { get; }
        protected abstract IPublisher Publisher { get; }
        public abstract void Dispose();

        public virtual void Init(IActorRef notifyActor)
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearData(autoTestGridDomainConfiguration);
            GridDomainNode.ConfigureLog(autoTestGridDomainConfiguration);

            var system = CreateSystem(AkkaConfig);
            var container = new UnityContainer();
            system.AddDependencyResolver(new UnityDependencyResolver(container, system));

            InitContainer(container, notifyActor);
            var routingActor = CreateRoutingActor(system);

            Router = new ActorMessagesRouter(routingActor,container.Resolve<IAggregateActorLocator>());
        }

        protected abstract IActorRef CreateRoutingActor(ActorSystem system);

        protected virtual void InitContainer(UnityContainer container, IActorRef actor)
        {
            container.RegisterType<IHandler<TestMessage>, TestHandler>(new InjectionConstructor(actor));
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            container.RegisterType<IAggregateActorLocator, DefaultAggregateActorLocator>();
            container.RegisterInstance(Subscriber);
            container.RegisterInstance(Publisher);
        }

        public void Publish(object[] commands)
        {
            foreach (var c in commands)
                Publisher.Publish(c);
        }

        public T[] WaitFor<T>(TestKit kit, int number)
        {
            var resultMessages = new List<T>();
            for (var num = 0; num < number; num++)
                resultMessages.Add(kit.ExpectMsg<T>(TimeSpan.FromSeconds(10)));

            return resultMessages.ToArray();
        }

        protected abstract ActorSystem CreateSystem(AkkaConfiguration conf);
    }
}