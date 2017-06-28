using GridDomain.CQRS;

namespace GridDomain.Node.Configuration.Composition {
    public interface IMessageHandlerWithMetadataFactory<TMessage, THandler>: IRouteMapFactory where THandler : IHandlerWithMetadata<TMessage>
    {
        THandler Create(IMessageProcessContext context);
    }
}