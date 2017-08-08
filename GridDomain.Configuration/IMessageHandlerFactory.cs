using GridDomain.CQRS;

namespace GridDomain.Configuration {
    public interface IMessageHandlerFactory<TMessage, THandler>: IRouteMapFactory where THandler : IHandler<TMessage>
    {
        THandler Create(IMessageProcessContext context);
    }
}