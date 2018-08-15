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

        public static void RegisterAggregate<TCommandAggregate>(this IDomainBuilder builder) where TCommandAggregate:CommandAggregate
        {
            builder.RegisterAggregate(AggregateDependencies.ForCommandAggregate<TCommandAggregate>());
        }

        public static void Register(this IDomainBuilder builder, IEnumerable<IDomainConfiguration> maps)
        {
            Register(builder, maps.ToArray());
        }

        public static HandlerRegistrator<IMessageProcessContext,TMessage, THandler> RegisterHandler<TMessage, THandler>(this IDomainBuilder builder) where THandler : IHandler<TMessage>, new()
                                                                                            where TMessage : class, IHaveProcessId, IHaveId
        {
            return new HandlerRegistrator<IMessageProcessContext,TMessage, THandler>(c => new THandler(), builder);
        }
        public static HandlerRegistrator<IMessageProcessContext,TMessage, THandler> RegisterHandler<TMessage, THandler>(this IDomainBuilder builder, Func<IMessageProcessContext, THandler> producer) where THandler : IHandler<TMessage>
                                                                                                                                where TMessage : class, IHaveProcessId, IHaveId
        {
            return new HandlerRegistrator<IMessageProcessContext,TMessage, THandler>(producer, builder);
        }
        
    }
}