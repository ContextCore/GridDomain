using GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
     //   void Register<TMessage, THandler>(Func<TMessage, THandler> handlerFactory) where THandler : IHandler<TMessage>;

        IRouteBuilder<TMessage> Route<TMessage>();
    }
}