namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    public class MessageProcessPolicy
    {
        public MessageProcessPolicy(bool isSynchronious)
        {
            IsSynchronious = isSynchronious;
        }

        //no other messages from chain should be processed until current will be processed
        public bool IsSynchronious { get; }

        public static MessageProcessPolicy Sync { get; } = new MessageProcessPolicy(true);
    }
}