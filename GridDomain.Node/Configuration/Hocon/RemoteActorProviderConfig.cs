using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon {
    internal class RemoteActorProviderConfig : IHoconConfig
    {
        public Config Build()
        {
            return @"akka.actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""";
        }
    }
}