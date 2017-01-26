using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Persistence;
using Akka.TestKit.Xunit2;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka.Hocon;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public abstract class NodeCommandsTest : TestKit
    {
        protected virtual bool CreateNodeOnEachTest { get; } = false;
        public NodeTestFixture NodeTestFixture { get; }
        protected virtual TimeSpan DefaultTimeout => NodeTestFixture.DefaultTimeout;
        protected NodeCommandsTest(ITestOutputHelper output, NodeTestFixture fixture): base(fixture.GetConfig(), fixture.Name)
        {
            Serilog.Log.Logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
            NodeTestFixture = fixture;
            NodeTestFixture.ExternalSystem = Sys;
        }

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine();
            Console.WriteLine("Stopping node");
            NodeTestFixture.Dispose();
            base.Dispose(disposing);
        }

        protected override void AfterAll()
        {
            if (!CreateNodeOnEachTest) return;
            
            base.AfterAll();
        }

        /// <summary>
        /// Loads aggregate using Sys actor system
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T LoadAggregate<T>(Guid id) where T : AggregateBase
        {
            var name = AggregateActorName.New<T>(id).ToString();
            var actor = LoadActor<AggregateActor<T>>(name).Result;
            return (T)actor.State;
        }

        private async Task<T> LoadActor<T>(string name) where T : ActorBase
        {
            var props = NodeTestFixture.ExternalSystem.DI().Props<T>();

            var actor = ActorOfAsTestActorRef<T>(props, name);

            await actor.Ask<RecoveryCompleted>(NotifyOnPersistenceEvents.Instance)
                       .TimeoutAfter(DefaultTimeout, $"Cannot load actor {typeof(T)}, id = {name}")
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
    }
}