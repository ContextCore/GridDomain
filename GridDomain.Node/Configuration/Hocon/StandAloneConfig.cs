namespace GridDomain.Node.Configuration.Hocon
{
    internal class StandAloneConfig : RemoteActorConfig
    {
        public StandAloneConfig(INodeNetworkAddress address) : base(address) {}

        public override string BuildActorProvider()
        {
            return @"
            actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            ";
        }
    }
}