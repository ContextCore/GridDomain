
using CommonDomain.Core;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        IRouteBuilder<TMessage> Route<TMessage>();

        void Register<TAggregate, TCommandHandler>(TCommandHandler handler) where TCommandHandler : AggregateCommandsHandler<TAggregate>
                                                                            where TAggregate : AggregateBase;
    }
}