using Akka.Actor;

namespace GridDomain.Node.Actors.PersistentHub {
    public class WarmUpResult
    {
        public WarmUpResult(IActorRef info, bool wasCreated)
        {
            Info = info;
            WasCreated = wasCreated;
        }
        public IActorRef Info { get; }
        public bool WasCreated { get; }
    }
}