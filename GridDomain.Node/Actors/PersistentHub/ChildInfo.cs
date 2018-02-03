using System;
using System.Collections.Generic;
using Akka.Actor;

namespace GridDomain.Node.Actors.PersistentHub
{
    public class ChildInfo
    {
        public DateTime ExpiresAt;
        public DateTime LastTimeOfAccess;
        public bool Terminating;
        public List<MessageWithSender> PendingMessages { get; } = new List<MessageWithSender>();
        public ChildInfo(IActorRef actor)
        {
            Ref = actor;
        }

        public IActorRef Ref { get; }

        public class MessageWithSender
        {
            public MessageWithSender(object message, IActorRef sender)
            {
                Message = message;
                Sender = sender;
            }
            public object Message { get; }
            public IActorRef Sender { get; }
        }
    }
}