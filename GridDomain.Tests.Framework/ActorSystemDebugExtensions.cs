using System;
using System.Threading.Tasks;
using Akka.Actor;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
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

        public static async Task KillAggregate<TAggregate>(this ActorSystem system,Guid id) where TAggregate:Aggregate
        {
            var aggregateHubActor = await system.LookupAggregateHubActor<TAggregate>();
            var aggregateActor = await system.LookupAggregateActor<TAggregate>(id);

            using (var inbox = Inbox.Create(system))
            {
                inbox.Watch(aggregateActor);
                aggregateHubActor.Tell(new ShutdownChild(id));
                inbox.ReceiveWhere(m => m is Terminated);
            }
        }

        public static async Task KillSaga<TSaga,TSagaData>(this ActorSystem system, Guid id) where TSagaData : ISagaState
        {
            var sagaHub = await system.LookupSagaHubActor<TSaga>();
            var saga = await system.LookupSagaActor<TSaga,TSagaData>(id);

            using (var inbox = Inbox.Create(system))
            {
                inbox.Watch(saga);
                sagaHub.Tell(new ShutdownChild(id));
                inbox.ReceiveWhere(m => m is Terminated);
            }
        }

        public static async Task<IActorRef> LookupSagaActor<TSaga, TData>(this ActorSystem system, Guid id) where TData : ISagaState
        {
            var sagaName = AggregateActorName.New<SagaStateAggregate<TData>>(id).Name;
            var sagaType = typeof(TSaga).BeautyName();

            return await system.ResolveActor($"akka://LocalSystem/user/{sagaType}_Hub/{sagaName}");
        }
        public static async Task<IActorRef> LookupSagaHubActor<TSaga>(this ActorSystem system)// where TSaga : //ISagaState
        {
            var sagaType = typeof(TSaga).BeautyName();
            return await system.ResolveActor($"akka://LocalSystem/user/{sagaType}_Hub");
        }
        public static async Task<IActorRef> ResolveActor(this ActorSystem system,string actorPath, TimeSpan? timeout = null)
        {
            return await system.ActorSelection(actorPath).ResolveOne(timeout ?? TimeSpan.FromSeconds(3));
        }

     
    }
}