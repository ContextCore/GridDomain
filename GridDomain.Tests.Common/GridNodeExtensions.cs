using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;

namespace GridDomain.Tests.Common
{
    public static class GridNodeExtensions
    {
        public static async Task<TExpect> SendToSaga<TExpect>(this GridDomainNode node, DomainEvent msg, TimeSpan? timeout = null) where TExpect : class
        {
            var res = await node.NewDebugWaiter(timeout)
                                .Expect<TExpect>()
                                .Create()
                                .SendToSagas(msg);

            return res.Message<TExpect>();
        }

        public static async Task<TState> GetTransitedState<TState>(this GridDomainNode node, DomainEvent msg, TimeSpan? timeout = null) where TState : ISagaState
        {
            var res = await node.SendToSaga<SagaReceivedMessage<TState>>(msg,timeout);
            return res.State;
        }

        public static async Task<TState> GetCreatedState<TState>(this GridDomainNode node, DomainEvent msg, TimeSpan? timeout = null) where TState : ISagaState
        {
            var res = await node.SendToSaga<SagaCreated<TState>>(msg, timeout);
            return res.State;
        }

        public static IMessageWaiter<AnyMessagePublisher> NewDebugWaiter(this GridDomainNode node, TimeSpan? timeout = null)
        {
            var conditionBuilder = new MetadataConditionBuilder<AnyMessagePublisher>();
            var waiter = new LocalMessagesWaiter<AnyMessagePublisher>(node.System, node.Transport, timeout ?? node.Settings.DefaultTimeout, conditionBuilder);
            conditionBuilder.CreateResultFunc = t => new AnyMessagePublisher(node.Pipe, waiter);
            return waiter;
        }

        public static async Task<TAggregate> LoadAggregate<TAggregate>(this GridDomainNode node, Guid id)
            where TAggregate : AggregateBase
        {
            using (var eventsRepo = new ActorSystemEventRepository(node.System))
            using (var repo = new AggregateRepository(eventsRepo))
            {
                return await repo.LoadAggregate<TAggregate>(id);
            }
        }

        public static async Task<object[]> LoadFromJournal(this GridDomainNode node, string id)
        {
            using (var repo = new ActorSystemJournalRepository(node.System))
            {
                return await repo.Load(id);
            }
        }

        public static async Task SaveToJournal(this GridDomainNode node, string id, params object[] messages)
        {
            using (var repo = new ActorSystemJournalRepository(node.System, false))
            {
                await repo.Save(id, messages);
            }
        }

        public static async Task SaveToJournal<TAggregate>(this GridDomainNode node, TAggregate aggregate) where TAggregate : Aggregate
        {
            var domainEvents = ((IAggregate) aggregate).GetUncommittedEvents()
                                                       .Cast<DomainEvent>()
                                                       .ToArray();

            await node.SaveToJournal<TAggregate>(aggregate.Id, domainEvents);

            aggregate.PersistAll();
        }

        public static async Task SaveToJournal<TAggregate>(this GridDomainNode node, Guid id, params DomainEvent[] messages)
            where TAggregate : AggregateBase
        {
            var name = AggregateActorName.New<TAggregate>(id).Name;
            await node.SaveToJournal(name, messages);
        }

        public static async Task<IActorRef> LookupAggregateActor<T>(this GridDomainNode node,
                                                                    Guid id,
                                                                    TimeSpan? timeout = null) where T : IAggregate
        {
            var name = AggregateActorName.New<T>(id).Name;
            return await node.ResolveActor($"{typeof(T).Name}_Hub/{name}", timeout);
        }

        public static async Task<IActorRef> LookupAggregateHubActor<T>(this GridDomainNode node, TimeSpan? timeout = null)
            where T : IAggregate
        {
            return await node.ResolveActor($"{typeof(T).Name}_Hub", timeout);
        }

        public static async Task KillAggregate<TAggregate>(this GridDomainNode node, Guid id, TimeSpan? timeout = null)
            where TAggregate : Aggregate
        {
            var aggregateHubActor = await node.LookupAggregateHubActor<TAggregate>(timeout);
            var aggregateActor = await node.LookupAggregateActor<TAggregate>(id, timeout);

            using (var inbox = Inbox.Create(node.System))
            {
                inbox.Watch(aggregateActor);
                aggregateHubActor.Tell(new ShutdownChild(id));
                inbox.ReceiveWhere(m => m is Terminated, timeout ?? node.Settings.DefaultTimeout);
            }
        }

        public static async Task KillSaga<TSaga, TSagaData>(this GridDomainNode node, Guid id, TimeSpan? timeout = null)
            where TSagaData : ISagaState
        {
            var sagaHub = await node.LookupSagaHubActor<TSaga>(timeout);
            var saga = await node.LookupSagaActor<TSaga, TSagaData>(id, timeout);

            using (var inbox = Inbox.Create(node.System))
            {
                inbox.Watch(saga);
                sagaHub.Tell(new ShutdownChild(id));
                inbox.ReceiveWhere(m => m is Terminated, timeout ?? node.Settings.DefaultTimeout);
            }
        }

        public static async Task<IActorRef> LookupSagaActor<TSaga, TData>(this GridDomainNode node,
                                                                          Guid id,
                                                                          TimeSpan? timeout = null) where TData : ISagaState
        {
            var sagaName = AggregateActorName.New<TData>(id).Name;
            var sagaType = typeof(TSaga).BeautyName();
            return await node.ResolveActor($"{sagaType}_Hub/{sagaName}", timeout);
        }

        public static async Task<IActorRef> LookupSagaHubActor<TSaga>(this GridDomainNode node, TimeSpan? timeout = null)
        {
            var sagaType = typeof(TSaga).BeautyName();
            return await node.ResolveActor($"{sagaType}_Hub", timeout);
        }

        public static async Task<IActorRef> ResolveActor(this GridDomainNode node, string actorPath, TimeSpan? timeout = null)
        {
            return
                await
                    node.System.ActorSelection("akka://LocalSystem/user/" + actorPath)
                        .ResolveOne(timeout ?? node.Settings.DefaultTimeout);
        }
    }
}