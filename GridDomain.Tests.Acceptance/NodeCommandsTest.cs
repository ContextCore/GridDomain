using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.TestKit.NUnit;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;
using GridDomain.Tests.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    public abstract class NodeCommandsTest : TestKit
    {
        protected static readonly AkkaConfiguration AkkaConf = new AutoTestAkkaConfiguration();
        protected GridDomainNode GridNode;
        private Stopwatch watch = new Stopwatch();
        private IActorSubscriber _actorSubscriber;

        protected NodeCommandsTest(string config, string name = null) : base(config, name)
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
            _actorSubscriber = GridNode.Container.Resolve<IActorSubscriber>();

        }

        public T LoadAggregate<T>(Guid id) where T : AggregateBase
        {
            var props = GridNode.System.DI().Props<AggregateActor<T>>();
            var name = AggregateActorName.New<T>(id).ToString();
            var actor = ActorOfAsTestActorRef<AggregateActor<T>>(props, name);
            Thread.Sleep(1000); //wait for actor recover
            return actor.UnderlyingActor.Aggregate;
        }

        protected abstract GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig);

        protected void ExecuteAndWaitFor<TEvent>(params ICommand[] commands)
        {
            var actor = GridNode.System
                                .ActorOf(Props.Create(() => new CountEventWaiter<TEvent>(commands.Length, TestActor)),
                                         "EventCounter_" + Guid.NewGuid());

            _actorSubscriber.Subscribe<TEvent>(actor);

            Console.WriteLine("Starting execute");

            var commandTypes = commands.Select(c => c.GetType())
                                       .GroupBy(c => c.Name)
                                       .Select(g => new {Name = g.Key, Count = g.Count()});

            foreach (var commandStat in commandTypes)
            {
                Console.WriteLine($"Executing {commandStat.Count} of {commandStat.Name}");
            }

            watch.Restart();

            foreach (var c in commands)
            {
                GridNode.Execute(c);
            }

            Console.WriteLine();
            Console.WriteLine($"Execution finished, wait started with timeout {Timeout}");

            var msg = FishForMessage(m => m is ExpectedMessagesRecieved<TEvent>,Timeout);
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Wait ended, total wait time: {watch.Elapsed}");
        }
    }
}