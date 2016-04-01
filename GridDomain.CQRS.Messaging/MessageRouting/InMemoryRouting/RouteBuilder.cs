namespace GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting
{
    public class RouteBuilder<TMessage> : IRouteBuilder<TMessage>
    {
        private readonly InMemoryMessagesRouter _router;

        public RouteBuilder(InMemoryMessagesRouter router)
        {
            _router = router;
        }

        public IHandlerBuilder<TMessage, THandler> To<THandler>() where THandler : IHandler<TMessage>
        {
            return new HandlerBuilder<TMessage, THandler>(_router);
        }
    } 
}