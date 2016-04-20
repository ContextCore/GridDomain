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
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    public class ActorSystemInfrastruture: IDisposable
    {
        public ActorSystem System;
        public AkkaPublisher Publisher;
        public ActorMessagesRouter Router;
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


        public void Publish(object[] commands)
        {
            foreach (var c in commands)
                Publisher.Publish(c);
        }

        public TestMessage[] WaitFor(TestKit kit, int number)
        {
            var resultMessages = new List<TestMessage>();
            for (int num = 0; num < number; num++)
                resultMessages.Add(kit.ExpectMsg<TestMessage>(TimeSpan.FromSeconds(10)));

            return resultMessages.ToArray();
        }

        public virtual void Dispose()
        {
            System.Terminate();
            System.Dispose();
        }
    }
}