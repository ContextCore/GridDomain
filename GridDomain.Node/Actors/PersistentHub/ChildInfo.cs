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
        public List<object> PendingMessages { get; } = new List<object>();
        public ChildInfo(IActorRef actor)
        {
            Ref = actor;
        }

        public IActorRef Ref { get; }
    }
}