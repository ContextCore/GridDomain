using System;
using System.Threading.Tasks;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tools.Repositories;

namespace GridDomain.Tests.XUnit
{
    public static class ActorSystemDebugExtensions
    {
        public static async Task<IActorRef> LookupAggregateActor<T>(this ActorSystem system, Guid id) where T : IAggregate
        {
            var name = AggregateActorName.New<T>(id).Name;
            return await system.ResolveActor($"akka://LocalSystem/user/Aggregate_{typeof(T).Name}/{name}");
        }

        public static async Task<IActorRef> LookupAggregateHubActor<T>(this ActorSystem system, string pooled) where T : IAggregate
        {
            return await system.ResolveActor($"akka://LocalSystem/user/Aggregate_{typeof(T).Name}");
        }

        public static async Task<IActorRef> LookupSagaActor<TSaga, TData>(this ActorSystem system, Guid id) where TData : ISagaState
        {
            var sagaName = AggregateActorName.New<SagaStateAggregate<TData>>(id).Name;
            var sagaType = typeof(TSaga).BeautyName();

            return await system.ResolveActor($"akka://LocalSystem/user/{sagaType}/{sagaName}");
        }
        public static async Task<IActorRef> ResolveActor(this ActorSystem system,string actorPath, TimeSpan? timeout = null)
        {
            return await system.ActorSelection(actorPath).ResolveOne(timeout ?? TimeSpan.FromSeconds(3));
        }

        public static async Task SaveToJournal<TAggregate>(this ActorSystem system, Guid id, params DomainEvent[] messages) where TAggregate : AggregateBase
        {
            string persistId = AggregateActorName.New<TAggregate>(id).ToString();
            var persistActor = system.ActorOf(
                Props.Create(() => new EventsRepositoryActor(persistId)), Guid.NewGuid().ToString());

            foreach (var o in messages)
                await persistActor.Ask<EventsRepositoryActor.Persisted>(new EventsRepositoryActor.Persist(o));
        }
    }
}