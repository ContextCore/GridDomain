using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Configuration.Composition {
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

        public static void RegisterHandler<TMessage, THandler>(this IDomainBuilder builder) where THandler : IHandler<TMessage>, new()
                                                                                            where TMessage : DomainEvent
        {
            RegisterHandler<TMessage, THandler>(builder, c => new THandler(), e => e.SourceId);
        }

        public static void RegisterMetadataHandler<TMessage, THandler>(this IDomainBuilder builder) where THandler : IHandlerWithMetadata<TMessage>, new()
                                                                                                    where TMessage : DomainEvent
        {
            RegisterMetadataHandler<TMessage, THandler>(builder, c => new THandler(), e => e.SourceId);
        }

        public static void RegisterHandler<TMessage, THandler>(this IDomainBuilder builder, Func<IMessageProcessContext, THandler> producer, Expression<Func<TMessage, Guid>> propertyExp) where THandler : IHandler<TMessage>
                                                                                                                                                                                           where TMessage : class, IHaveSagaId, IHaveId
        {
            builder.RegisterHandler(new DefaultMessageHandlerFactory<TMessage, THandler>(producer,propertyExp));
        }

        public static void RegisterMetadataHandler<TMessage, THandler>(this IDomainBuilder builder, Func<IMessageProcessContext, THandler> producer, Expression<Func<TMessage, Guid>> propertyExp) 
            where THandler : IHandlerWithMetadata<TMessage>
            where TMessage : class, IHaveSagaId, IHaveId
        {
            builder.RegisterHandler(new DefaultMessageWithMetadataHandlerFactory<TMessage, THandler>(producer, propertyExp));
        }
    }
}