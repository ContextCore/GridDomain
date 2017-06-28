using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Persistence;
using Akka.TestKit.Xunit2;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Tests.Unit
{
    public static class TestKitExtensions
    {
        public static async Task<T> LoadAggregateByActor<T>(this TestKit kit, Guid id) where T : Aggregate
        {
            var name = AggregateActorName.New<T>(id).ToString();
            var actor = await kit.LoadActor<AggregateActor<T>>(name);
            return actor.State;
        }

        private static async Task<T> LoadActor<T>(this TestKit kit, string name) where T : ActorBase
        {
            var props = kit.Sys.DI().Props<T>();

            var actor = kit.ActorOfAsTestActorRef<T>(props, name);

            await actor.Ask<RecoveryCompleted>(NotifyOnPersistenceEvents.Instance,TimeSpan.FromSeconds(5));

            return actor.UnderlyingActor;
        }

        public static async Task<TSagaState> LoadSagaByActor<TSagaState>(this TestKit kit, Guid id)
            where TSagaState : class, ISagaState
        {
            return (await kit.LoadAggregateByActor<SagaStateAggregate<TSagaState>>(id)).State;
        }
    }
}