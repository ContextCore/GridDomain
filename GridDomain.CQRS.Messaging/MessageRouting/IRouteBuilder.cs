using CommonDomain.Core;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IRouteBuilder<TMessage>
    {
        IHandlerBuilder<TMessage, THandler> ToHandler<THandler>() where THandler : IHandler<TMessage>;
        ICommandRouteBuilder<TMessage, TAggregate> To<TAggregate>() where TAggregate : AggregateBase;
    }

    public interface ICommandRouteBuilder<TMessage, TAggregate> where TAggregate : AggregateBase
    {
        void Register();
    }
}