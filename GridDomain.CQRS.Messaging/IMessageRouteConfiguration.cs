using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public interface IMessageRouteConfiguration
    {
        void Register(IMessagesRouter bus);
    }
}