using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.TestKit.NUnit;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Logging;
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

        [TestFixtureTearDown]
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

        protected ExpectedMessagesRecieved ExecuteAndWaitFor<TEvent>(params ICommand[] commands)
        {
            return ExecuteAndWaitFor(new[] { typeof(TEvent) }, commands);
        }
        protected ExpectedMessagesRecieved ExecuteAndWaitFor<TMessage1, TMessage2>(params ICommand[] commands)
        {
            return ExecuteAndWaitFor(new[] { typeof(TMessage1), typeof(TMessage2) }, commands);
        }

        private ExpectedMessagesRecieved WaitForFirstOf(Action act, params Type[] messageTypes)
        {
            var toWait = messageTypes.Select(m => new MessageToWait(m, 1)).ToArray();
            var actor = GridNode.System
                                .ActorOf(Props.Create(() => new MessageWaiter(toWait, TestActor)),
                                         "MessageWaiter_" + Guid.NewGuid());

            foreach (var m in messageTypes)
                _actorSubscriber.Subscribe(m, actor);

            act();

            Console.WriteLine();
            Console.WriteLine($"Execution finished, wait started with timeout {Timeout}");

            var msg = (ExpectedMessagesRecieved)FishForMessage(m => m is ExpectedMessagesRecieved, Timeout);
            watch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Wait ended, total wait time: {watch.Elapsed}");
            Console.WriteLine("Stoped after message recieved:");
            Console.WriteLine("------begin of message-----");
            Console.WriteLine(msg.ToPropsString());
            Console.WriteLine("------end of message-----");
            return msg;
        }

        protected ExpectedMessagesRecieved ExecuteAndWaitFor(Type[] messageTypes, params ICommand[] commands)
        {
            return WaitForFirstOf(() => Execute(commands), messageTypes);
        }

        protected ExpectedMessagesRecieved WaitFor<TMessage>()
        {
            return WaitForFirstOf(() => { }, typeof(TMessage));
        }

        private void Execute(ICommand[] commands)
        {
            Console.WriteLine("Starting execute");

            var commandTypes = commands.Select(c => c.GetType())
                .GroupBy(c => c.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() });

            foreach (var commandStat in commandTypes)
            {
                Console.WriteLine($"Executing {commandStat.Count} of {commandStat.Name}");
            }

            watch.Restart();

            foreach (var c in commands)
            {
                GridNode.Execute(c);
            }
        }
    }
}