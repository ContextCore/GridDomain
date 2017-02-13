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
using GridDomain.Tools.Repositories.EventRepositories;

namespace GridDomain.Tests.Framework
{
    public static class ActorSystemDebugExtensions
    {
        public static async Task<IActorRef> LookupAggregateActor<T>(this ActorSystem system, Guid id) where T : IAggregate
        {
            var name = AggregateActorName.New<T>(id).Name;
            return await system.ResolveActor($"akka://LocalSystem/user/{typeof(T).Name}_Hub/{name}");
        }

        public static async Task<IActorRef> LookupAggregateHubActor<T>(this ActorSystem system) where T : IAggregate
        {
            return await system.ResolveActor($"akka://LocalSystem/user/{typeof(T).Name}_Hub");
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

     
    }
}