using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.TestKit;
using Akka.TestKit.NUnit;
using CommonDomain.Persistence;
using GridDomain.Balance.Node;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;
using NLog;
using NLog.LayoutRenderers.Wrappers;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    public class NodeCommandsTest: TestKit
    {
        protected GridDomainNode GridNode;
       // private IActorRef _distributedPubSub;
        private IActorSubscriber _subscriber;
        [TearDown]
        public void DeleteSystems()
        {
            Console.WriteLine();
            Console.WriteLine("Stopping node");
            GridNode.Stop();
        }

        [SetUp]
        protected void Init()
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(autoTestGridDomainConfiguration);

            AkkaConfiguration akkaConf = new AutoTestAkkaConfiguration();

            GridNode = GreateGridDomainNode(akkaConf, autoTestGridDomainConfiguration);

            GridNode.Start(autoTestGridDomainConfiguration);
            _subscriber = GridNode.Container.Resolve<IActorSubscriber>();

        }

        protected virtual GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var actorSystem = ActorSystemFactory.CreateActorSystem(akkaConf);

            return new GridDomainNode(DefaultUnityContainer(dbConfig), new BalanceCommandsRouting(),  actorSystem);
        }

        protected static UnityContainer DefaultUnityContainer(IDbConfiguration autoTestGridDomainConfiguration)
        {
            var unityContainer = new UnityContainer();
            GridDomain.Balance.Node.CompositionRoot.Init(unityContainer, autoTestGridDomainConfiguration);
            return unityContainer;
        }

        protected void ExecuteAndWaitFor<TEvent,TCommand>(TCommand[] commands,
                                                          Func<TCommand,Guid> expectedSource) 
                                                  where TEvent : ISourcedEvent
                                                  where TCommand:ICommand
        {
            var sources = commands.Select(expectedSource).ToArray();
            
            var actor = GridNode.System.ActorOf(Props.Create(
                                        () => new ExplicitSourcesEventWaiter<TEvent>(TestActor,sources)));

            _subscriber.Subscribe<TEvent>(actor);

            Console.WriteLine("Starting execute");

            foreach (var c in commands)
                GridNode.Execute(c);

            Console.WriteLine();
            Console.WriteLine($"Execution finished, wait started with timeout {Timeout}");

            ExpectMsg<ExpectedMessagesRecieved<TEvent>>(Timeout);
            Console.WriteLine();
            Console.WriteLine("Wait ended");
        }

        protected virtual TimeSpan Timeout => TimeSpan.FromSeconds(10);
    }
}