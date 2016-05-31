using CommonDomain.Core;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IRouteBuilder<TMessage>
    {
        IHandlerBuilder<TMessage, THandler> ToHandler<THandler>() where THandler : IHandler<TMessage>;
      //  void RegisterAggregateCommands<TMessage, TAggregate>() where TAggregate : AggregateBase
    }
}