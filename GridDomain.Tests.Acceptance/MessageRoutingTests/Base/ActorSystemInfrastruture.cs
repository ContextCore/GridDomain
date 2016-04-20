using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    class ActorSystemInfrastruture
    {
        public ActorSystem System;
        public AkkaPublisher Publisher;
        private ActorMessagesRouter Router;
        public readonly AkkaConfiguration AkkaConfig;


        public ActorSystemInfrastruture(AkkaConfiguration conf)
        {
            AkkaConfig = conf;
        }

        public virtual void Init(IActorRef notifyActor)
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(autoTestGridDomainConfiguration);
            GridDomainNode.ConfigureLog(autoTestGridDomainConfiguration);

            var container = new UnityContainer();
            var propsResolver = new UnityDependencyResolver(container, System);
            InitContainer(container, notifyActor);
            Router = new ActorMessagesRouter(System.ActorOf(System.DI().Props<AkkaRoutingActor>()));
            Publisher = new AkkaPublisher(System);
        }

        protected virtual void InitContainer(UnityContainer container, IActorRef actor)
        {
            container.RegisterType<IHandler<TestMessage>, TestHandler>(new InjectionConstructor(actor));
            container.RegisterType<IHandlerActorTypeFactory, DefaultHandlerActorTypeFactory>();
        }

        protected virtual void InitRouting()
        {
            
        }
    }
}