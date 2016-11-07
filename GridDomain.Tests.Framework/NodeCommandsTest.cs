using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Persistence;
using Akka.TestKit.NUnit3;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Logging;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Framework
{
    public abstract class NodeCommandsTest : TestKit
    {
        protected static readonly AkkaConfiguration AkkaConf = new AutoTestAkkaConfiguration();
        protected GridDomainNode GridNode;
      
        private readonly Stopwatch _watch = new Stopwatch();
        protected virtual bool ClearDataOnStart { get; } = false;
        protected virtual bool CreateNodeOnEachTest { get; } = false;

        protected NodeCommandsTest(string config, string name = null, bool clearDataOnStart = true) : base(config, name)
        {
            ClearDataOnStart = clearDataOnStart;
        }

        protected abstract TimeSpan Timeout { get; }

        [OneTimeTearDown]
        public void DeleteSystems()
        {
            if (CreateNodeOnEachTest) return;
            Console.WriteLine();
            Console.WriteLine("Stopping node");
            GridNode.Stop();
            Sys.Terminate();
        }

        protected override void AfterAll()
        {
            if (CreateNodeOnEachTest)
                GridNode.Stop();
        }

        [SetUp]
        public void CreateNode()
        {
            if (!CreateNodeOnEachTest) return;
            Start();
        }

        [OneTimeSetUp]
        public void Init()
        {
            if (CreateNodeOnEachTest) return;
            Start();
        }

        private void Start()
        {
            LogManager.SetLoggerFactory(new AutoTestLogFactory());

            var autoTestGridDomainConfiguration = new AutoTestLocalDbConfiguration();
            if (ClearDataOnStart)
                TestDbTools.ClearData(autoTestGridDomainConfiguration, AkkaConf.Persistence);

            GridNode = CreateGridDomainNode(AkkaConf, autoTestGridDomainConfiguration);
            GridNode.Start(autoTestGridDomainConfiguration);
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
            var actor = LoadActorByDI<AggregateActor<T>>(name);
            return actor.Aggregate;
        }

        private T LoadActorByDI<T>(string name) where T : ActorBase
        {
            var props = GridNode.System.DI().Props<T>();
            var actor = ActorOfAsTestActorRef<T>(props, name);
            actor.Ask<RecoveryCompleted>(NotifyOnRecoverComplete.Instance).Wait(Timeout);
            return actor.UnderlyingActor;
        }

        public TSagaState LoadSagaState<TSaga, TSagaState>(Guid id) where TSagaState : AggregateBase where TSaga : class, ISagaInstance
        {
            var name = AggregateActorName.New<TSagaState>(id).ToString();
            var actor = LoadActorByDI<SagaActor<TSaga, TSagaState>>(name);
            return (TSagaState)actor.Saga.Data;
        }
        public SagaDataAggregate<TSagaState> LoadInstanceSagaState<TSaga, TSagaState>(Guid id) where TSagaState : class, ISagaState
                                                                            where TSaga : Saga<TSagaState>
        {
            return LoadSagaState<ISagaInstance<TSaga,TSagaState>, SagaDataAggregate<TSagaState>>(id);
        }


        protected abstract GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig);

        private ExpectedMessage[] GetFaults(ICommand[] commands)
        {
            var faultGeneric = typeof(Fault<>);
            return commands.Select(c => c.GetType())
                           .Distinct()
                           .Select(commandType => faultGeneric.MakeGenericType(commandType))
                           .Select(t => new ExpectedMessage(t, 0))
                           .ToArray();
        }
        protected ExpectedMessagesReceived ExecuteAndWaitFor<TEvent>(params ICommand[] commands)
        {
            var messageTypes = GetFaults(commands).Concat(new[] { Expect.Message<TEvent>() }).ToArray();
            return ExecuteAndWaitFor(messageTypes, commands);
        }
        protected ExpectedMessagesReceived ExecuteAndWaitFor<TMessage1, TMessage2>(params ICommand[] commands)
        {
            var messageTypes = GetFaults(commands).Concat(new[] { Expect.Message<TMessage1>(), Expect.Message<TMessage1>() }).ToArray();
            return ExecuteAndWaitFor(messageTypes, commands);
        }
        protected ExpectedMessagesReceived ExecuteAndWaitForMany<TMessage1, TMessage2>(int eventAnum, int eventBnum, params ICommand[] commands)
        {
            var msg1ToWait = new ExpectedMessage(typeof(TMessage1), eventAnum);
            var msg2ToWait = new ExpectedMessage(typeof(TMessage2), eventBnum);
            var allMsgToWait = GetFaults(commands).Concat(new [] {msg1ToWait, msg2ToWait}).ToArray();

            return Wait(() => Execute(commands), GridNode.System, true, allMsgToWait);
        }


        private ExpectedMessagesReceived Wait(Action act, ActorSystem system, bool failOnCommandFault = true,  params ExpectedMessage[] expectedMessages)
        {
            var actor = system.ActorOf(Props.Create(() => new AllMessageWaiterActor(TestActor, expectedMessages)),
                                         "MessageWaiter_" + Guid.NewGuid());
            var actorSubscriber= GridNode.Container.Resolve<IActorSubscriber>();

            foreach (var m in expectedMessages)
                actorSubscriber.Subscribe(m.MessageType, actor);

            act();

            Console.WriteLine();
            Console.WriteLine($"Execution finished, wait started with timeout {Timeout}");

            var msg = (ExpectedMessagesReceived) FishForMessage(m => m is ExpectedMessagesReceived, Timeout);
            _watch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Wait ended, total wait time: {_watch.Elapsed}");
            Console.WriteLine("Stopped after message received:");
            Console.WriteLine("------begin of message-----");
            Console.WriteLine(msg.ToPropsString());
            Console.WriteLine("------end of message-----");

            if (failOnCommandFault && msg.Message is IFault)
            {
                Assert.Fail($"Command fault received: {msg.ToPropsString()}");
            }

            return msg;
        }

        protected ExpectedMessagesReceived ExecuteAndWaitFor(Type[] messageTypes, params ICommand[] commands)
        {
            return Wait(() => Execute(commands), GridNode.System,true, messageTypes.Select(m => new ExpectedMessage(m,1)).ToArray());
        }

        protected ExpectedMessagesReceived ExecuteAndWaitFor(ExpectedMessage[] expectedMessage, params ICommand[] commands)
        {
            return Wait(() => Execute(commands), GridNode.System, true, expectedMessage);
        }

        protected ExpectedMessagesReceived WaitFor<TMessage>(bool failOnFault = true)
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

            ((ICommandExecutor)GridNode).Execute(commands);
        }


        protected void SaveToJournal(params object[] messages)
        {
            var persistenceExtension = Persistence.Instance.Get(GridNode.System) ?? Persistence.Instance.Apply(GridNode.System);

            var settings = persistenceExtension.Settings;
            var journal = persistenceExtension.JournalFor(null);

            int seqNumber = 0;
            var envelop =
                messages.Select(e => new Akka.Persistence.AtomicWrite(
                             new Persistent(e, seqNumber++, "testId", e.GetType()
                                                                      .AssemblyQualifiedShortName())))
                      .Cast<IPersistentEnvelope>()
                      .ToArray();

            var writeMsg = new WriteMessages(envelop, TestActor, 1);

            journal.Tell(writeMsg);

            FishForMessage<WriteMessagesSuccessful>(m => true);
        }


        protected IEnumerable<object> LoadFromJournal(string persistenceId, int expectedCount)
        {
            var persistenceExtension = Persistence.Instance.Get(GridNode.System) ?? Persistence.Instance.Apply(GridNode.System);
            var settings = persistenceExtension.Settings;
            var journal = persistenceExtension.JournalFor(null);

            var loadMsg = new ReplayMessages(0, long.MaxValue, long.MaxValue, persistenceId, TestActor);

            journal.Tell(loadMsg);

            for (int i = 0; i < expectedCount; i++)
                yield return FishForMessage<ReplayedMessage>(m => m.Persistent.PersistenceId == persistenceId).Persistent.Payload;
        }

    }
}