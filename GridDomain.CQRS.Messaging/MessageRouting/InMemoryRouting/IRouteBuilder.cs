namespace GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting
{
    public interface IRouteBuilder<TMessage>
    {
        IHandlerBuilder<TMessage, THandler> To<THandler>() where THandler : IHandler<TMessage>;
    }
}