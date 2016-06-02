namespace GridDomain.Node.Configuration.Hocon
{
    internal class StandAloneConfig : ActorConfig
    {
        public StandAloneConfig(IAkkaNetworkAddress address) : base(address)
        {
        }

        public override string BuildActorProvider()
        {
            var clusterConfigString =
                @"
            actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            ";

            return clusterConfigString;
        }
    }
}