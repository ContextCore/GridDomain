using System;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public class WaitEndInfo : EventArgs
    {
        public WaitEndInfo(IWaitResult result, object lastMessage)
        {
            Result = result;
            LastMessage = lastMessage;
        }

        public IWaitResult Result { get; }
        public object LastMessage { get; }
    }
}