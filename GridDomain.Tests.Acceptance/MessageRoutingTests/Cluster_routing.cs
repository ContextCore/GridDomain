using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.MessageRoutingTests
{
    
    class Cluster_routing_correlated:Given_correlated_routing
    {
        private ActorSystem[] _clusterSystems;

        class ClusterMessage : TestMessage
        {
            public Address ProcessorActorSystemAdress { get; set; }
        }

        class ClusterMessageHandlerActor  : MessageHandlingActor<ClusterMessage, TestHandler> 
        {
            public ClusterMessageHandlerActor(TestHandler handler) : base(handler)
            {
            }

            protected override void OnReceive(object msg)
            {
                ((ClusterMessage) msg).ProcessorActorSystemAdress = Cluster.Get(Context.System).SelfAddress;
                base.OnReceive(msg);
            }
        }

        class ClusterActorTypeFactory : IHandlerActorTypeFactory
        {
            public Type GetActorTypeFor(Type message, Type handler)
            {
                return typeof (ClusterMessageHandlerActor);
            }
        }


        [TearDown]
        public void Clear()
        {
            foreach (var system in _clusterSystems)
            {
                system.Terminate();
                system.Dispose();
            }
        }

        protected override void InitContainer(UnityContainer container)
        {
            base.InitContainer(container);
            container.RegisterType<IHandlerActorTypeFactory, ClusterActorTypeFactory>();
        }

        [Test]
        public void Messages_should_be_processed_by_remote_nodes()
        {
            Assert.True(_resultMessages.All( m => ((ClusterMessage)m).ProcessorActorSystemAdress != Cluster.Get(_system).SelfAddress));
        }
    }

}
