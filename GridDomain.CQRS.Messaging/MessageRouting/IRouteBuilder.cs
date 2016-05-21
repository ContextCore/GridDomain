using CommonDomain.Core;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IRouteBuilder<TMessage>
    {
        IHandlerBuilder<TMessage, THandler> To<THandler>() where THandler : IHandler<TMessage>;
        IAggregateCommandRouteBuilder<TMessage, TAggregate> ToAggregate<TAggregate>() where TAggregate : AggregateBase;
    }

    public interface IAggregateCommandRouteBuilder<TMessage, TAggregate> where TAggregate : AggregateBase
    {
        void Register();
    }
}