namespace GridDomain.Node.Akka.Configuration.Hocon {
    internal class RemoteActorProviderConfig : IHoconConfig
    {
        public string Build()
        {
            return @"akka.actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""";
        }
    }
}