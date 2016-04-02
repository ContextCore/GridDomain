using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.TestKit.NUnit;
using CommonDomain.Persistence;
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
    public class NodeCommandsTest : TestKit
    {
        protected GridDomainNode GridNode;

        [TearDown]
        public void DeleteSystems()
        {
            Console.WriteLine("Stopping node");
            GridNode.Stop();
        }

        [SetUp]
        protected void Init()
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(autoTestGridDomainConfiguration);

            GridNode = new GridDomainNode(new AkkaConfiguration("LocalSystem", 8000,"127.0.0.1", "ERROR"),
                                          autoTestGridDomainConfiguration,
                                          new UnityContainer());

            GridNode.Start();
        }

        protected void ExecuteAndWaitFor<TEvent,TCommand>(TCommand[] commands,
                                                          Func<TCommand,Guid> expectedSource) 
                                                  where TEvent : ISourcedEvent
                                                  where TCommand:ICommand
        {
            var sources = commands.Select(expectedSource).ToArray();
            
            var actor = Sys.ActorOf(Props.Create(
                                    () => new ExplicitSourcesEventWaiter<TEvent>(TestActor,sources)));

            DistributedPubSub.Get(GridNode.System)
                             .Mediator
                             .Ask(new Subscribe(typeof(TEvent).FullName, actor))
                             .Wait(TimeSpan.FromSeconds(5));

            foreach(var c in commands)
                GridNode.Execute(c);

            Console.WriteLine($"Wait started with timeout {Timeout}");

            ExpectMsg<ExpectedMessagesRecieved<TEvent>>(Timeout);
            Console.WriteLine("Wait ended");
        }

        protected virtual TimeSpan Timeout => TimeSpan.FromSeconds(100);
    }
}