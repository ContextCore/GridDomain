using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    public static class GridNodeDebugExtensions
    {
        public static Task<T> ExecuteCorrelated<T>(this IGridDomainNode node, ICommand command) where T:DomainEvent
        {
            return node.Execute(command, ExpectedMessage.Once<T>(t => t.SourceId,command.Id));
        }
    }
}
