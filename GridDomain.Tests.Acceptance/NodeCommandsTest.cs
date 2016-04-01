using System;
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
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    public class NodeCommandsTest : TestKit
    {
        protected GridDomainNode GridNode;

        [TearDown]
        public void DeleteSystems()
        {
            GridNode.Stop();
        }

        [SetUp]
        protected void Init()
        {
            var autoTestGridDomainConfiguration = TestEnvironment.Configuration;
            TestDbTools.ClearAll(autoTestGridDomainConfiguration);
            GridNode = new GridDomainNode(new AkkaConfiguration("LocalSystem", 8000,"127.0.0.1"),
                                          autoTestGridDomainConfiguration,
                                          new UnityContainer());
            GridNode.Start();
        }

        protected void ExecuteAndWaitFor<T>(ICommand command,
                                            Guid sources) where T : ISourcedEvent
        {
            ExecuteAndWaitFor<T>(command, new[] { sources }, DistributedPubSub.Get(GridNode.System)
                .Mediator, 1);
        }
        protected void ExecuteAndWaitFor<T>(ICommand command,
                                            Guid[] sources, IActorRef Mediator,
                                            int msgNum = 0) where T : ISourcedEvent
        {
            var numLeft = msgNum == 0 ? sources.Length : msgNum;

            var actor = Sys.ActorOf(Props.Create(
                                    () => new EventWaiter<T>(TestActor, numLeft, sources)));

            Mediator
                             .Ask(new Subscribe(typeof(T).FullName, actor))
                             .Wait(TimeSpan.FromSeconds(1));

            GridNode.Execute(command);

            ExpectMsg<ExpectedMessagesRecieved<T>>(Timeout);
        }

        protected virtual TimeSpan Timeout => TimeSpan.FromSeconds(100);
    }
}