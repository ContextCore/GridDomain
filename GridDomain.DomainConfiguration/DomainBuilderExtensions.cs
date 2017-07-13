using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Configuration {
    public static class DomainBuilderExtensions
    {
        public static void Register(this IDomainBuilder builder, params IDomainConfiguration[] maps)
        {
            foreach(var m in maps)
                m.Register(builder);
        }
        public static void Register(this IDomainBuilder builder, IEnumerable<IDomainConfiguration> maps)
        {
            Register(builder, maps.ToArray());
        }

        public static HandlerRegistrator<TMessage, THandler> RegisterHandler<TMessage, THandler>(this IDomainBuilder builder) where THandler : IHandler<TMessage>, new()
                                                                                            where TMessage : class, IHaveSagaId, IHaveId
        {
            return new HandlerRegistrator<TMessage, THandler>(c => new THandler(), builder);
        }
        public static HandlerRegistrator<TMessage, THandler> RegisterHandler<TMessage, THandler>(this IDomainBuilder builder, Func<IMessageProcessContext, THandler> producer) where THandler : IHandler<TMessage>
                                                                                                                                where TMessage : class, IHaveSagaId, IHaveId
        {
            return new HandlerRegistrator<TMessage, THandler>(producer, builder);
        }
    }
}