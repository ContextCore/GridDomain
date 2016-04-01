using System;
using GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class HandlerBuilder<TMessage, THandler> : IHandlerBuilder<TMessage, THandler>
        where THandler : IHandler<TMessage>
    {
        private readonly InMemoryMessagesRouter _router;
        private Func<TMessage, THandler> _factory;

        public HandlerBuilder(InMemoryMessagesRouter router)
        {
            _router = router;
        }

        public void Register()
        {
            _router.Register(_factory);
        }

        public HandlerBuilder<TMessage, THandler> WithFactory(Func<TMessage, THandler> factory)
        {
            _factory = factory;
            return this;
        }
    }
}