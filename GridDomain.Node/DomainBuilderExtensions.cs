using System;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node {
    public static class DomainBuilderExtensions
    {
        public static HandlerRegistrator<INodeContext,TMessage, THandler> RegisterNodeHandler<TMessage, THandler>(this IDomainBuilder builder, Func<INodeContext, THandler> producer) where THandler : IHandler<TMessage>
                                                                                                                                                                                      where TMessage : class, IHaveProcessId, IHaveId
        {
            return new HandlerRegistrator<INodeContext,TMessage, THandler>(producer, builder);
        }
    }
}