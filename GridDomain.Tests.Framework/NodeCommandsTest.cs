using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.TestKit.NUnit;
using Automatonymous;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Serilog.Events;

namespace GridDomain.Tests.Framework
{
    public abstract class NodeCommandsTest : TestKit
    {
        protected static readonly AkkaConfiguration AkkaConf = new AutoTestAkkaConfiguration();
        protected GridDomainNode GridNode;
      
        private readonly Stopwatch _watch = new Stopwatch();
        private IActorSubscriber _actorSubscriber;
        private readonly bool _clearDataOnStart;

        protected NodeCommandsTest(string config, string name = null, bool clearDataOnStart = true) : base(config, name)
        {
            _clearDataOnStart = clearDataOnStart;
        }

        protected abstract TimeSpan Timeout { get; }

        [TestFixtureTearDown]
        public void DeleteSystems()
        {
            Console.WriteLine();
            Console.WriteLine("Stopping node");
            GridNode.Stop();
            Sys.Terminate();
        }

        //do not terminate actor system after each [Test] run
        protected override void AfterAll()
        {
        }

        [TestFixtureSetUp]
        protected void Init()
        {
            LogManager.SetLoggerFactory(new DefaultLoggerFactory(new AutoTestLogConfig(LogEventLevel.Verbose)));
            
            var autoTestGridDomainConfiguration = new AutoTestLocalDbConfiguration();
            if (_clearDataOnStart)
                TestDbTools.ClearData(autoTestGridDomainConfiguration, AkkaConf.Persistence);

            GridNode = CreateGridDomainNode(AkkaConf, autoTestGridDomainConfiguration);
            GridNode.Start(autoTestGridDomainConfiguration);
            _actorSubscriber = GridNode.Container.Resolve<IActorSubscriber>();

        }

        /// <summary>
        /// Loads aggregate using Sys actor system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T LoadAggregate<T>(Guid id) where T : AggregateBase
        {
            var name = AggregateActorName.New<T>(id).ToString();
            return LoadAggregate<T>(name);
        }

        public T LoadAggregate<T>(string name) where T : AggregateBase
        {
            var props = GridNode.System.DI().Props<AggregateActor<T>>();
            var actor = ActorOfAsTestActorRef<AggregateActor<T>>(props, name);
            //TODO: replace with event wait
            Thread.Sleep(1000); //wait for actor recover
            return actor.UnderlyingActor.Aggregate;
        }

        public TSagaState LoadSagaState<TSaga, TSagaState, TStartMessage>(Guid id) where TStartMessage : DomainEvent where TSagaState : AggregateBase where TSaga : class, ISagaInstance
        {
            var props = GridNode.System.DI().Props<SagaActor<TSaga, TSagaState, TStartMessage>>();
            var name = AggregateActorName.New<TSagaState>(id).ToString();
            var actor = ActorOfAsTestActorRef<SagaActor<TSaga, TSagaState, TStartMessage>>(props, name);
            Thread.Sleep(1000); //wait for actor recover
            return (TSagaState)actor.UnderlyingActor.Saga.Data;
        }

        protected abstract GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig);

        private ExpectedMessage[] GetFaults(ICommand[] commands)
        {
            var faultGeneric = typeof(CommandFault<>);
            return commands.Select(c => c.GetType())
                           .Distinct()
                           .Select(commandType => faultGeneric.MakeGenericType(commandType))
                           .Select(t => new ExpectedMessage(t, 0))
                           .ToArray();
        }
        protected ExpectedMessagesRecieved ExecuteAndWaitFor<TEvent>(params ICommand[] commands)
        {
            var messageTypes = GetFaults(commands).Concat(new[] { ExpectedMessage.Once<TEvent>() }).ToArray();
            return ExecuteAndWaitFor(messageTypes, commands);
        }
        protected ExpectedMessagesRecieved ExecuteAndWaitFor<TMessage1, TMessage2>(params ICommand[] commands)
        {
            var messageTypes = GetFaults(commands).Concat(new[] { ExpectedMessage.Once<TMessage1>(), ExpectedMessage.Once<TMessage1>() }).ToArray();
            return ExecuteAndWaitFor(messageTypes, commands);
        }
        protected ExpectedMessagesRecieved ExecuteAndWaitForMany<TMessage1, TMessage2>(int eventAnum, int eventBnum, params ICommand[] commands)
        {
            var msg1ToWait = new ExpectedMessage(typeof(TMessage1), eventAnum);
            var msg2ToWait = new ExpectedMessage(typeof(TMessage2), eventBnum);
            var allMsgToWait = GetFaults(commands).Concat(new [] {msg1ToWait, msg2ToWait}).ToArray();

            return Wait(() => Execute(commands), GridNode.System, true, allMsgToWait);
        }


        private ExpectedMessagesRecieved Wait(Action act, ActorSystem system, bool failOnCommandFault = true,  params ExpectedMessage[] expectedMessages)
        {
            var actor = system
                                .ActorOf(Props.Create(() => new AllMessageWaiter(TestActor, expectedMessages)),
                                         "MessageWaiter_" + Guid.NewGuid());

            foreach (var m in expectedMessages)
                _actorSubscriber.Subscribe(m.MessageType, actor);

            act();

            Console.WriteLine();
            Console.WriteLine($"Execution finished, wait started with timeout {Timeout}");

            var msg = (ExpectedMessagesRecieved)FishForMessage(m => m is ExpectedMessagesRecieved, Timeout);
            _watch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Wait ended, total wait time: {_watch.Elapsed}");
            Console.WriteLine("Stopped after message received:");
            Console.WriteLine("------begin of message-----");
            Console.WriteLine(msg.ToPropsString());
            Console.WriteLine("------end of message-----");

            if (failOnCommandFault && msg.Message is ICommandFault)
            {
                Assert.Fail($"Command fault received: {msg.ToPropsString()}");
            }

            return msg;
        }

        protected ExpectedMessagesRecieved ExecuteAndWaitFor(Type[] messageTypes, params ICommand[] commands)
        {
            return Wait(() => Execute(commands), GridNode.System,true, messageTypes.Select(m => new ExpectedMessage(m,1)).ToArray());
        }

        protected ExpectedMessagesRecieved ExecuteAndWaitFor(ExpectedMessage[] expectedMessage, params ICommand[] commands)
        {
            return Wait(() => Execute(commands), GridNode.System, true, expectedMessage);
        }

        protected ExpectedMessagesRecieved WaitFor<TMessage>(bool failOnFault = true)
        {
            return Wait(() => { }, GridNode.System, failOnFault, new ExpectedMessage(typeof(TMessage), 1));
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

            _watch.Restart();

            foreach (var c in commands)
            {
                GridNode.Execute(c);
            }
        }
    }
}