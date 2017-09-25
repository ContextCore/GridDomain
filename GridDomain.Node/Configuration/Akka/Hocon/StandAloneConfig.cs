namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class StandAloneConfig : RemoteActorConfig
    {
        public StandAloneConfig(IAkkaNetworkAddress address) : base(address) {}

        public override string BuildActorProvider()
        {
            return @"
            actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            ";
        }
    }
}