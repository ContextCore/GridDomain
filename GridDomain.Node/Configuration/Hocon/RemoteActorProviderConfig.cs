using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon {
    internal class RemoteActorProviderConfig : IHoconConfig
    {
        public string Build()
        {
            return @"akka.actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""";
        }
    }
}