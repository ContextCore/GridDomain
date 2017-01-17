namespace GridDomain.Node.Actors
{
    public class MessageProcessPolicy
    {
        public MessageProcessPolicy(bool isSynchronious)
        {
            IsSynchronious = isSynchronious;
        }

        //no other messages from chain should be processed until current will be processed
        public bool IsSynchronious { get; }     
    }
}