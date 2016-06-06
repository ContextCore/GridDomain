using System;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using CompositionRoot = GridDomain.Balance.Node.CompositionRoot;

namespace GridDomain.Tests.Acceptance
{
    public abstract class NodeCommandsTest : TestKit
    {
        protected static readonly AkkaConfiguration AkkaConf = new AutoTestAkkaConfiguration();
        private IActorSubscriber _subscriber;
        protected GridDomainNode GridNode;

        protected NodeCommandsTest(string config) : base(config)
        {
        }

        protected abstract TimeSpan Timeout { get; }

        [TearDown]
        public void DeleteSystems()
        {
            Console.WriteLine();
            Console.WriteLine("Stopping node");
            GridNode.Stop();
        }

        [TestFixtureSetUp]
        protected void Init()
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;

            TestDbTools.ClearData(autoTestGridDomainConfiguration, AkkaConf.Persistence);

            GridNode = GreateGridDomainNode(AkkaConf, autoTestGridDomainConfiguration);
            GridNode.Start(autoTestGridDomainConfiguration);
            _subscriber = GridNode.Container.Resolve<IActorSubscriber>();
        }

        protected abstract GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig);

        protected static UnityContainer DefaultUnityContainer(IDbConfiguration autoTestGridDomainConfiguration)
        {
            var unityContainer = new UnityContainer();
            CompositionRoot.Init(unityContainer, autoTestGridDomainConfiguration);
            return unityContainer;
        }

        protected void ExecuteAndWaitFor<TEvent>(ICommand[] commands, int eventNumber)
        {
            var actor = Sys.ActorOf(Props.Create(() => new CountEventWaiter<TEvent>(eventNumber, TestActor)));

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
    }
}