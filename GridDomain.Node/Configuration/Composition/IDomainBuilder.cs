using System;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Configuration.Composition
{
    public interface IDomainConfiguration
    {
        void Register(IDomainBuilder builder);
    }

    public interface IDomainBuilder
    {
        void RegisterSaga<TState, TProcess>(ISagaDependencyFactory<TProcess, TState> factory) where TProcess : Process<TState>
                                                                                              where TState : class, ISagaState;

        void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate;

        void RegisterHandler<TMessage, THandler>(IMessageHandlerFactory<TMessage, THandler> factory) where THandler : IHandler<TMessage>;
        void RegisterHandler<TMessage, THandler>(IMessageHandlerWithMetadataFactory<TMessage, THandler> factory) where THandler : IHandlerWithMetadata<TMessage>;
    }

    class DelegateMessageHandlerFactory<TMessage, THandler> : IMessageHandlerFactory<TMessage, THandler> where THandler : IHandler<TMessage>
    {
        private readonly Func<IMessageProcessContext, THandler> _creator;

        public DelegateMessageHandlerFactory(Func<IMessageProcessContext, THandler> creator)
        {
            _creator = creator;
        }

        public THandler Create(IMessageProcessContext context)
        {
            return _creator(context);
        }
    }

    class DelegateMessageWithMetadataHandlerFactory<TMessage, THandler> : IMessageHandlerWithMetadataFactory<TMessage, THandler> where THandler : IHandlerWithMetadata<TMessage>
    {
        private readonly Func<IMessageProcessContext, THandler> _creator;

        public DelegateMessageWithMetadataHandlerFactory(Func<IMessageProcessContext, THandler> creator)
        {
            _creator = creator;
        }

        public THandler Create(IMessageProcessContext context)
        {
            return _creator(context);
        }
    }

    public static class DomainBuilderExtensions
    {
        public static void Register(this IDomainBuilder builder, IDomainConfiguration cfg)
        {
            cfg.Register(builder);
        }

        public static void RegisterHandler<TMessage, THandler>(this IDomainBuilder builder) where THandler : IHandler<TMessage>, new()
        {
            RegisterHandler<TMessage, THandler>(builder, c => new THandler());
        }

        public static void RegisterMetadataHandler<TMessage, THandler>(this IDomainBuilder builder) where THandler : IHandlerWithMetadata<TMessage>, new()
        {
            RegisterMetadataHandler<TMessage, THandler>(builder, c => new THandler());
        }

        public static void RegisterHandler<TMessage, THandler>(this IDomainBuilder builder, Func<IMessageProcessContext, THandler> producer) where THandler : IHandler<TMessage>
        {
            builder.RegisterHandler(new DelegateMessageHandlerFactory<TMessage, THandler>(producer));
        }

        public static void RegisterMetadataHandler<TMessage, THandler>(this IDomainBuilder builder, Func<IMessageProcessContext, THandler> producer) where THandler : IHandlerWithMetadata<TMessage>
        {
            builder.RegisterHandler(new DelegateMessageWithMetadataHandlerFactory<TMessage, THandler>(producer));
        }
    }

    public interface IMessageProcessContext
    {
        IPublisher Publisher { get; }
    }

    public interface IMessageHandlerFactory<TMessage, THandler> where THandler : IHandler<TMessage>
    {
        THandler Create(IMessageProcessContext context);
    }

    public interface IMessageHandlerWithMetadataFactory<TMessage, THandler> where THandler : IHandlerWithMetadata<TMessage>
    {
        THandler Create(IMessageProcessContext context);
    }
}