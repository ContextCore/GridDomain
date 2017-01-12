using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace Shop.Composition
{
    public class ShopRouteMap : IMessageRouteMap
    {
        public Task Register(IMessagesRouter router)
        {
            throw new System.NotImplementedException();
        }
    }
}