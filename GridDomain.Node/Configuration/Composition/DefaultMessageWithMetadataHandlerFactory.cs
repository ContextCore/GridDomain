using System;
using System.Linq.Expressions;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Composition {
    class DefaultMessageWithMetadataHandlerFactory<TMessage, THandler> : MessageHandlerFactory<TMessage, THandler>,
                                                                         IMessageHandlerWithMetadataFactory<TMessage, THandler> where THandler : IHandlerWithMetadata<TMessage>
                                                                                                                                where TMessage : class, IHaveSagaId, IHaveId
    {
        public DefaultMessageWithMetadataHandlerFactory(Func<IMessageProcessContext, THandler> creator, Expression<Func<TMessage, Guid>> expression) : base(creator, expression) {
            
        }
    }
}