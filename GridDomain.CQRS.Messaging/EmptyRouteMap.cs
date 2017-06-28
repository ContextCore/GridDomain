using System.Threading.Tasks;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public class EmptyRouteMap : IMessageRouteMap
    {
        public EmptyRouteMap(string name = null)
        {
            Name = name ?? nameof(EmptyRouteMap);
        }
        public Task Register(IMessagesRouter router)
        {
            return Task.CompletedTask;
        }

        public string Name { get; }

        public static EmptyRouteMap Instance { get; } = new EmptyRouteMap();
    }
}