using System;
using System.Linq.Expressions;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Composition {
    class DefaultMessageHandlerFactory<TMessage, THandler> : MessageHandlerFactory<TMessage, THandler>,
                                                             IMessageHandlerFactory<TMessage, THandler> where THandler : IHandler<TMessage>
                                                                                                        where TMessage : class, IHaveSagaId, IHaveId
    {
        public DefaultMessageHandlerFactory(Func<IMessageProcessContext, THandler> creator, Expression<Func<TMessage, Guid>> expression) : base(creator, expression)
        {

        }
    }
}