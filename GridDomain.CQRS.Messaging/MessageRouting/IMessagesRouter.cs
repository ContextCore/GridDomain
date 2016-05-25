
namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        IRouteBuilder<TMessage> Route<TMessage>();
    }
}