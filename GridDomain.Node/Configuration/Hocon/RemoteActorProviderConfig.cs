using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon {
    internal class RemoteActorProviderConfig : IHoconConfig
    {
        public Config Build()
        {
            return @"actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""";
        }
    }
}