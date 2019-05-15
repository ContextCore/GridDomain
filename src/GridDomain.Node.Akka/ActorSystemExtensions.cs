using Akka.Actor;

namespace GridDomain.Node.Akka {
    public static class ActorSystemExtensions
    {
        public static Address GetAddress(this ActorSystem sys)
        {
            return ((ExtendedActorSystem) sys).Provider?.DefaultAddress;
        }

        public static string GetDefaultLogFileName(this ActorSystem s)
        {
            var address = s.GetAddress();
            string port;
         
             port = address.Port.ToString();

            return $"{s.Name}_{port}";
        }
    }
}