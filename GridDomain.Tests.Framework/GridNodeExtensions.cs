using System;
using System.Threading.Tasks;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;

namespace GridDomain.Tests.Framework
{
    public static class GridNodeExtensions
    {
        public static IMessageWaiter<AnyMessagePublisher> NewDebugWaiter(this GridDomainNode node, TimeSpan? timeout = null)
        {
            return new DebugLocalWaiter(node.Pipe, node.System, node.Transport,timeout ??  node.Settings.DefaultTimeout);
        }

       public static async Task<TAggregate> LoadAggregate<TAggregate>(this GridDomainNode node, Guid id) where TAggregate : AggregateBase
       {
           using (var eventsRepo = new ActorSystemEventRepository(node.System))
           using (var repo = new AggregateRepository(eventsRepo))
           {
               return await repo.LoadAggregate<TAggregate>(id);
           }
       }
       
       public static async Task SaveToJournal<TAggregate>(this GridDomainNode node, Guid id, params DomainEvent[] messages) where TAggregate : AggregateBase
       {
           await node.System.SaveToJournal<TAggregate>(id, messages);
       }
    }
}