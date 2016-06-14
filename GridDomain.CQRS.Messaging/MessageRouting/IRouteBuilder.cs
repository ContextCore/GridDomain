namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IRouteBuilder<TMessage>
    {
        //TODO: refactor to mapping per handler, not per event
        IHandlerBuilder<TMessage, THandler> ToHandler<THandler>() where THandler : IHandler<TMessage>;
    }
}