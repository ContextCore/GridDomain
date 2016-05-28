using GridDomain.Node.Configuration;

class StandAloneConfig : ActorConfig
{
    public StandAloneConfig(IAkkaNetworkAddress address) : base(address.PortNumber, address.Name)
    {
    }

    public override string BuildActorProvider()
    {
        string clusterConfigString =
            @"
            actor.provider =  ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
            }";

        return clusterConfigString;
    }
}