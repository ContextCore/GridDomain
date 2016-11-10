using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class WaitEndInfo : EventArgs
    {
        public WaitEndInfo(IWaitResults results, object lastMessage)
        {
            Results = results;
            LastMessage = lastMessage;
        }

        public IWaitResults Results { get; }
        public object LastMessage { get; }
    }
}