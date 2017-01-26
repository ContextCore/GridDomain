using System.Threading.Tasks;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public class EmptyRouteMap : IMessageRouteMap
    {
        public Task Register(IMessagesRouter router)
        {
            return Task.CompletedTask;
        }
    }
}