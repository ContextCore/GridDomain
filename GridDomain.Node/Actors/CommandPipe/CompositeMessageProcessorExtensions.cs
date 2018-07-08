using GridDomain.Node.Actors.CommandPipe.MessageProcessors;

namespace GridDomain.Node.Actors.CommandPipe {
    public static class CompositeMessageProcessorExtensions
    {
        public static void Add<TMessage>(this ICompositeMessageProcessor proc, IMessageProcessor messageProcessor)
        {
            proc.Add(typeof(TMessage), messageProcessor);
        }
    }
}