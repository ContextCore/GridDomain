using Akka.Actor;

namespace GridDomain.Node.Cluster {
    public static class ActorSystemClusterExtensions
    {
        public static Address GetAddress(this ActorSystem sys)
        {
            return ((ExtendedActorSystem) sys).Provider.DefaultAddress;
        }
    }
}