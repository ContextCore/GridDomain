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
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode.SingleSystem.Setup;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.Balance.MessageRoutingTests.GridNode
{
    public abstract class ActorSystemInfrastruture: IDisposable
    {
        public ActorSystem System { get; private set; }
        public IPublisher Publisher;
        public ActorMessagesRouter Router;
        public readonly AkkaConfiguration AkkaConfig;


        protected ActorSystemInfrastruture(AkkaConfiguration conf)
        {
            AkkaConfig = conf;
        }

        public virtual void Init(IActorRef notifyActor)
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(autoTestGridDomainConfiguration);
            GridDomainNode.ConfigureLog(autoTestGridDomainConfiguration);

            System = CreateSystem(AkkaConfig);
            var container = new UnityContainer();
            var propsResolver = new UnityDependencyResolver(container, System);
            InitContainer(container, notifyActor);
            Router = new ActorMessagesRouter(System.ActorOf(System.DI().Props<AkkaRoutingActor>()),
                                             container.Resolve<IAggregateActorLocator>());

            Publisher = new DistributedPubSubTransport(System);
        }

        protected virtual void InitContainer(UnityContainer container, IActorRef actor)
        {
            container.RegisterType<IHandler<TestMessage>, TestHandler>(new InjectionConstructor(actor));
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
            container.RegisterType<IAggregateActorLocator,DefaultAggregateActorLocator>();
        }


        public void Publish(object[] commands)
        {
            foreach (var c in commands)
                Publisher.Publish(c);
        }
        
        public T[] WaitFor<T>(TestKit kit, int number)
        {
            var resultMessages = new List<T>();
            for (int num = 0; num < number; num++)
                resultMessages.Add(kit.ExpectMsg<T>(TimeSpan.FromSeconds(10)));

            return resultMessages.ToArray();
        }

        protected abstract ActorSystem CreateSystem(AkkaConfiguration conf);

        public virtual void Dispose()
        {
            System.Terminate();
            System.Dispose();
        }
    }
}