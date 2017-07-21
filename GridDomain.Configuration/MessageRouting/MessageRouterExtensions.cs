using System.Threading.Tasks;
using GridDomain.EventSourcing;

namespace GridDomain.Configuration.MessageRouting
{
    public static class MessageRouterExtensions
    {
        //TODO: add version without correlation property

        public static Task RegisterAggregate<TAggregate, TCommandHandler>(this IMessagesRouter router)
            where TAggregate : Aggregate where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
        {
            return RegisterAggregate(router, new TCommandHandler());
        }

        public static Task RegisterAggregate<TAggregate>(this IMessagesRouter router,
                                                         AggregateCommandsHandler<TAggregate> handler)
            where TAggregate : Aggregate
        {
            var descriptor = new AggregateCommandsHandlerDescriptor<TAggregate>();
            foreach (var info in handler.RegisteredCommands)
                descriptor.RegisterCommand(info);

            return router.RegisterAggregate(descriptor);
        }
    }
}