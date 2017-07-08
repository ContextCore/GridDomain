using System;
using Akka.Actor;

namespace GridDomain.Node.Actors.PersistentHub
{
    public class ChildInfo
    {
        public DateTime ExpiresAt;
        public DateTime LastTimeOfAccess;
        public bool Terminating;

        public ChildInfo(IActorRef actor)
        {
            Ref = actor;
        }

        public IActorRef Ref { get; }
    }
}