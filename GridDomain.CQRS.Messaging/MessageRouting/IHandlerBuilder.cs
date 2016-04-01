namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IHandlerBuilder<TMessage, THandler> where THandler : IHandler<TMessage>
    {
        // HandlerBuilder<TMessage, THandler> WithFactory(Func<TMessage, THandler> factory);
        void Register();
    }
}