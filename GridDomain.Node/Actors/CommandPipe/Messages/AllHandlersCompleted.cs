using System;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class AllHandlersCompleted
    {
        private AllHandlersCompleted()
        {
        }
        public static AllHandlersCompleted Instance { get; } = new AllHandlersCompleted();
    }
}