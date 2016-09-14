using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public interface IMessageRouteMap
    {
        void Register(IMessagesRouter router);
    }
    
}