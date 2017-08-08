namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class StandAloneConfig : RemoteActorConfig
    {
        public StandAloneConfig(IAkkaNetworkAddress address) : base(address) {}

        public override string BuildActorProvider()
        {
            var clusterConfigString = @"
            actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            ";

            return clusterConfigString;
        }
    }
}