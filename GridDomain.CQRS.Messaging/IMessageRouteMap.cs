using System.Threading.Tasks;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public interface IMessageRouteMap
    {
        Task Register(IMessagesRouter router);
    }
    
}