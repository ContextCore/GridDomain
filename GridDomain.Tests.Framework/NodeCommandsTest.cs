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
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools.Repositories;
using Helios.Util;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Serilog;

namespace GridDomain.Tests.Framework
{

    public abstract class NodeCommandsTest : TestKit
    {
        protected static readonly AkkaConfiguration AkkaConf = new AutoTestAkkaConfiguration();
        protected GridDomainNode GridNode;
        private CancellationTokenSource _additionalLogCancellationTokenSource;
        private bool _startLogging = true;
        protected virtual bool LogOnStartup { get; } = false;
        protected virtual bool ClearDataOnStart { get; } = false;
        protected virtual bool CreateNodeOnEachTest { get; } = false;
        protected virtual bool InMemory { get; } = true;
        protected static readonly AkkaConfiguration AkkaCfg = new AutoTestAkkaConfiguration();
        protected IPublisher Publisher => GridNode.Transport;

        static NodeCommandsTest()
        {
            Serilog.Log.Logger = new AutoTestLoggerConfiguration().CreateLogger();
        }

        protected NodeCommandsTest(string config, string name = null, bool clearDataOnStart = true) : base(config, name)
        {
            ClearDataOnStart = clearDataOnStart;
        }
        protected NodeCommandsTest(bool inMemory = true, AkkaConfiguration cfg = null) : 
            this( inMemory ? (cfg ?? AkkaCfg).ToStandAloneInMemorySystemConfig() : (cfg ?? AkkaCfg).ToStandAloneSystemConfig()
                , AkkaCfg.Network.SystemName
                , !inMemory)
        {
            InMemory = inMemory;
        }

        protected virtual GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(CreateConfiguration(), CreateMap(), () => new[] { Sys },
                (InMemory ? (IQuartzConfig)new InMemoryQuartzConfig() : new PersistedQuartzConfig()));
        }
        protected virtual IContainerConfiguration CreateConfiguration()
        {
            return new EmptyContainerConfiguration();
        }

        protected virtual IMessageRouteMap CreateMap()
        {
            return new CustomRouteMap();
        }

        protected virtual TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(3);

        [OneTimeTearDown]
        public async Task DeleteSystems()
        {
            if (CreateNodeOnEachTest) return;
            Console.WriteLine();
            Console.WriteLine("Stopping node");
            await GridNode.Stop();
        }

        protected async Task<IActorRef> LookupAggregateActor<T>(Guid id) where T: IAggregate
        {
           var name = AggregateActorName.New<T>(id).Name;
           return await ResolveActor($"akka://LocalSystem/user/Aggregate_{typeof(T).Name}/{name}");
        }

        protected async Task<IActorRef> LookupAggregateHubActor<T>(string pooled) where T: IAggregate
        {
           return await ResolveActor($"akka://LocalSystem/user/Aggregate_{typeof(T).Name}");
        }
        
        protected async Task<IActorRef> LookupSagaActor<TSaga,TData>(Guid id) where TData: ISagaState
        {
            var sagaName = AggregateActorName.New<SagaStateAggregate<TData>>(id).Name;
            var sagaType = typeof(TSaga).BeautyName();

            return await ResolveActor($"akka://LocalSystem/user/{sagaType}/{sagaName}");
        }
        private async Task<IActorRef> ResolveActor(string actorPath, TimeSpan? timeout = null)
        {
            return await GridNode.System.ActorSelection(actorPath)
                                        .ResolveOne(timeout ?? DefaultTimeout);
        }


        protected override void AfterAll()
        {
            if (!CreateNodeOnEachTest) return;
            _additionalLogCancellationTokenSource.Cancel();
            GridNode.Stop().Wait(DefaultTimeout);

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
            var autoTestGridDomainConfiguration = new AutoTestLocalDbConfiguration();
            if (ClearDataOnStart)
                TestDbTools.ClearData(AkkaConf.Persistence);

            GridNode = CreateGridDomainNode(AkkaConf);
            OnNodeCreated();
            await GridNode.Start();
            OnNodeStarted();
        }

        protected virtual void OnNodeCreated(){}

        //NUnit3 runner has a weird problem not passing logs from actor-invoked console.WriteLine 
        //to runner output. It is a hack to get logs from system
        private void AttachAdditionalLog()
        {
            var inbox = Inbox.Create(GridNode.System);
            GridNode.System.EventStream.Subscribe(inbox.Receiver, typeof(Akka.Event.Debug));
            GridNode.System.EventStream.Subscribe(inbox.Receiver, typeof(Akka.Event.Info));
            GridNode.System.EventStream.Subscribe(inbox.Receiver, typeof(Akka.Event.Warning));
            GridNode.System.EventStream.Subscribe(inbox.Receiver, typeof(Akka.Event.Error));
            _additionalLogCancellationTokenSource = new CancellationTokenSource();

            Task.Run(() =>
            {
                while (!_additionalLogCancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        var log = inbox.Receive();
                        Console.WriteLine(log);
                        TestContext.Out.WriteLine(log);
                        TestContext.Progress.WriteLine(log);
                    }
                    catch 
                    {
                        //inentionally left blank
                    }
                }

                GridNode?.System?.EventStream?.Unsubscribe(inbox.Receiver, typeof(Akka.Event.Debug));
                GridNode?.System?.EventStream?.Unsubscribe(inbox.Receiver, typeof(Akka.Event.Info));
                GridNode?.System?.EventStream?.Unsubscribe(inbox.Receiver, typeof(Akka.Event.Warning));
                GridNode?.System?.EventStream?.Unsubscribe(inbox.Receiver, typeof(Akka.Event.Error));

                inbox.Dispose();

            }, _additionalLogCancellationTokenSource.Token);
        }

        protected void StartLog()
        {
            _startLogging = true;
        }

        protected virtual void OnNodeStarted()
        {
            AttachAdditionalLog();
            StartLog();
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
            var actor = LoadActor<AggregateActor<T>>(name).Result;
            return (T)actor.State;
        }

        private async Task<T> LoadActor<T>(string name) where T : ActorBase
        {
            var props = GridNode.System.DI().Props<T>();
           
            var actor = ActorOfAsTestActorRef<T>(props, name);

            await actor.Ask<RecoveryCompleted>(NotifyOnPersistenceEvents.Instance)
                       .TimeoutAfter(DefaultTimeout,$"Cannot load actor {typeof(T)}, id = {name}")
                       .ConfigureAwait(false);

            return actor.UnderlyingActor;
        }

        public async Task<SagaStateAggregate<TSagaState>> LoadSaga<TSaga, TSagaState>(Guid id) where TSagaState : class, ISagaState
                                                                            where TSaga : Saga<TSagaState>
        {
            var name = AggregateActorName.New<SagaStateAggregate<TSagaState>>(id).ToString();
            var actor = await LoadActor<SagaActor<ISagaInstance<TSaga, TSagaState>, SagaStateAggregate<TSagaState>>>(name);
            return actor.Saga.Data;
        }


        protected virtual async Task SaveToJournal<TAggregate>(Guid id, params DomainEvent[] messages) where TAggregate : AggregateBase
        {
            string persistId = AggregateActorName.New<TAggregate>(id).ToString();
            var persistActor = GridNode.System.ActorOf(
                Props.Create(() => new EventsRepositoryActor(persistId)), Guid.NewGuid().ToString());

            foreach (var o in messages)
                await persistActor.Ask<EventsRepositoryActor.Persisted>(new EventsRepositoryActor.Persist(o));
        }

        protected void SaveToJournalDirectly(params object[] messages)
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


        protected IEnumerable<object> LoadFromJournalDirectly(string persistenceId, int expectedCount)
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