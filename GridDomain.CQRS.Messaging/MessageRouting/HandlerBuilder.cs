using System;
using GridDomain.CQRS.Messaging.MessageRouting.InMemoryRouting;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class HandlerBuilder<TMessage, THandler> : IHandlerBuilder<TMessage, THandler> where THandler : IHandler<TMessage>
    {
        private Func<TMessage, THandler> _factory;
        private readonly InMemoryMessagesRouter _router;

        public HandlerBuilder(InMemoryMessagesRouter router)
        {
            _router = router;
        }

        public HandlerBuilder<TMessage, THandler> WithFactory(Func<TMessage, THandler> factory)
        {
            _factory = factory;
            return this;
        }

        public void Register()
        {
            _router.Register(_factory);
        }
    }
}