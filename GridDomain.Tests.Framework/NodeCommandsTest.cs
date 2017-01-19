using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Persistence;
using Akka.TestKit.NUnit3;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.Common;
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
        public async Task DeleteSystems()
        {
            if (CreateNodeOnEachTest) return;
            Console.WriteLine();
            Console.WriteLine("Stopping node");
            await GridNode.Stop();
        }

        protected IActorRef LookupAggregateActor<T>(Guid id) where T: IAggregate
        {
           var name = AggregateActorName.New<T>(id).Name;
           return ResolveActor($"akka://LocalSystem/user/Aggregate_{typeof(T).Name}/{name}");
        }
        protected IActorRef LookupAggregateHubActor<T>(string pooled) where T: IAggregate
        {
           return ResolveActor($"akka://LocalSystem/user/Aggregate_{typeof(T).Name}");
        }

        private IActorRef ResolveActor(string actorPath)
        {
            return GridNode.System.ActorSelection(actorPath)
                                  .ResolveOne(Timeout)
                                  .Result;
        }

        protected IActorRef LookupSagaActor<TSaga,TData>(Guid id) where TData: ISagaState
        {
            var sagaName = AggregateActorName.New<SagaStateAggregate<TData>>(id).Name;
            var sagaType = typeof(TSaga).BeautyName();

            return GetSagaActor(sagaType, sagaName);
        }

        private IActorRef GetSagaActor(string sagaType, string sagaName)
        {
            return ResolveActor($"akka://LocalSystem/user/{sagaType}/{sagaName}");
        }

        protected override void AfterAll()
        {
            if (!CreateNodeOnEachTest) return;
            GridNode.Stop().Wait(Timeout);
            base.AfterAll();
        }

        [SetUp]
        public async Task CreateNode()
        {
            if (!CreateNodeOnEachTest) return;
            await Start();
        }

        [OneTimeSetUp]
        public async Task Init()
        {
            if (CreateNodeOnEachTest) return;
            await Start();
        }

        protected virtual async Task Start()
        {
            LogManager.SetLoggerFactory(new AutoTestLogFactory());

            var autoTestGridDomainConfiguration = new AutoTestLocalDbConfiguration();
            if (ClearDataOnStart)
                TestDbTools.ClearData(autoTestGridDomainConfiguration, AkkaConf.Persistence);

            GridNode = CreateGridDomainNode(AkkaConf);
            OnNodeCreated();
            await GridNode.Start();
            OnNodeStarted();
        }

        protected virtual void OnNodeCreated() { }
        protected virtual void OnNodeStarted() { }
        
        /// <summary>
        /// Loads aggregate using Sys actor system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T LoadAggregate<T>(Guid id) where T : AggregateBase
        {
            var name = AggregateActorName.New<T>(id).ToString();
            return LoadAggregate<T>(name).Result;
        }

        public async Task<T> LoadAggregate<T>(string name) where T : AggregateBase
        {
            var actor = await LoadActorByDI<AggregateActor<T>>(name);
            return (T)actor.State;
        }

        private async Task<T> LoadActorByDI<T>(string name) where T : ActorBase
        {
            var props = GridNode.System.DI().Props<T>();
           
            var actor = ActorOfAsTestActorRef<T>(props, name);

            await actor.Ask<RecoveryCompleted>(NotifyOnPersistenceEvents.Instance)
                       .TimeoutAfter(Timeout,$"Cannot load actor {typeof(T)}, id = {name}")
                       .ConfigureAwait(false);

            return actor.UnderlyingActor;
        }

        public TSagaState LoadSagaState<TSaga, TSagaState>(Guid id) where TSagaState : AggregateBase where TSaga : class, ISagaInstance
        {
            var name = AggregateActorName.New<TSagaState>(id).ToString();
            var actor = LoadActorByDI<SagaActor<TSaga, TSagaState>>(name).Result;
            return (TSagaState)actor.Saga.Data;
        }
        public SagaStateAggregate<TSagaState> LoadInstanceSagaState<TSaga, TSagaState>(Guid id) where TSagaState : class, ISagaState
                                                                            where TSaga : Saga<TSagaState>
        {
            return  LoadSagaState<ISagaInstance<TSaga,TSagaState>, SagaStateAggregate<TSagaState>>(id);
        }

        protected abstract GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf);


        protected void SaveToJournal(params object[] messages)
        {
            var persistenceExtension = Akka.Persistence.Persistence.Instance.Get(GridNode.System) ?? Akka.Persistence.Persistence.Instance.Apply(GridNode.System);

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
            var persistenceExtension = Akka.Persistence.Persistence.Instance.Get(GridNode.System) ?? Akka.Persistence.Persistence.Instance.Apply(GridNode.System);
            var settings = persistenceExtension.Settings;
            var journal = persistenceExtension.JournalFor(null);

            var loadMsg = new ReplayMessages(0, long.MaxValue, long.MaxValue, persistenceId, TestActor);

            journal.Tell(loadMsg);

            for (int i = 0; i < expectedCount; i++)
                yield return FishForMessage<ReplayedMessage>(m => m.Persistent.PersistenceId == persistenceId).Persistent.Payload;
        }

    }
}