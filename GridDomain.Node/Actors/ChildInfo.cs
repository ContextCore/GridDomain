using System;
using Akka.Actor;

namespace GridDomain.Node.Actors
{
    public class ChildInfo
    {
        public IActorRef Ref { get; }
        public DateTime  LastTimeOfAccess;

        public ChildInfo(IActorRef actor)
        {
            Ref = actor;
        }
    }
}