using GridDomain.CQRS.Messaging.MessageRouting;
using Microsoft.Practices.Unity;

namespace GridDomain.CQRS.Messaging
{
    public interface IMessageRouteMap
    {
        void Register(IMessagesRouter bus);
    }


}