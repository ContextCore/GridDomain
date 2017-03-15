using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Persistence;
using Akka.TestKit.Xunit2;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.XUnit
{
    public static class TestKitExtensions
    {
        public static async Task<T> LoadAggregate<T>(this TestKit kit, Guid id) where T : Aggregate
        {
            var name = AggregateActorName.New<T>(id).ToString();
            var actor = await kit.LoadActor<AggregateActor<T>>(name);
            return actor.State;
        }

        public static async Task<T> LoadActor<T>(this TestKit kit, string name) where T : ActorBase
        {
            var props = kit.Sys.DI().Props<T>();

            var actor = kit.ActorOfAsTestActorRef<T>(props, name);

            await actor.Ask<RecoveryCompleted>(NotifyOnPersistenceEvents.Instance);

            return actor.UnderlyingActor;
        }

        public static async Task<TSagaState> LoadSaga<TSaga, TSagaState>(this TestKit kit, Guid id)
            where TSagaState : class, ISagaState where TSaga : SagaStateMachine<TSagaState>
        {
            var name = AggregateActorName.New<SagaStateAggregate<TSagaState>>(id).ToString();
            var actor =
                await kit.LoadActor<SagaActor<TSaga, TSagaState>>(name);
            return actor.Saga.State;
        }
    }
}