using System;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Configuration {
    public class HandlerRegistrator<TContext,TMessage,THandler> where THandler : IHandler<TMessage>
                                                       where TMessage : class, IHaveProcessId, IHaveId
    {
        private readonly Func<TContext, THandler> _producer;
        private readonly IDomainBuilder _builder;

        public HandlerRegistrator(Func<TContext, THandler> producer, IDomainBuilder builder)
        {
            _producer = producer;
            _builder = builder;
        }

        public void AsSync()
        {
            _builder.RegisterHandler(new MessageHandlerFactory<TContext,TMessage, THandler>(_producer,() => 
                                                                                                 new CustomRouteMap(r => r.RegisterSyncHandler<TMessage,THandler>())));
        }

        public void AsFireAndForget()
        {
            _builder.RegisterHandler(new MessageHandlerFactory<TContext,TMessage, THandler>(_producer, () =>
                                                                                                  new CustomRouteMap(r => r.RegisterFireAndForgetHandler<TMessage, THandler>())));
        }
    }
}