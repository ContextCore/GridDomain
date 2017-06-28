using GridDomain.CQRS;

namespace GridDomain.Node.Configuration.Composition {
    public interface IMessageHandlerFactory<TMessage, THandler>: IRouteMapFactory where THandler : IHandler<TMessage>
    {
        THandler Create(IMessageProcessContext context);
    }
}