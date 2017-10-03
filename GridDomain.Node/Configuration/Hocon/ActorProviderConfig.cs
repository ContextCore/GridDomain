namespace GridDomain.Node.Configuration.Hocon {
    internal class ActorProviderConfig : IHoconConfig
    {
        public string Build()
        {
            return @"actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""";
        }
    }
}