using GridDomain.CQRS;

namespace GridDomain.Configuration {
    public interface IMessageHandlerFactory<TContext,TMessage, THandler>: IRouteMapFactory where THandler : IHandler<TMessage>
    {
        THandler Create(TContext context);
    }
}