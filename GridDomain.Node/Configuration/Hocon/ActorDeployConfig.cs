using GridDomain.Node.Configuration;

internal class ActorDeployConfig : IAkkaConfig
{
    private readonly AkkaConfiguration _akkaConf;

    public ActorDeployConfig(AkkaConfiguration akkaConf)
    {
        _akkaConf = akkaConf;
    }

    public static string BuildDeployConfig(AkkaConfiguration akkaConf)
    {
        string remoteConfig = @"
        remote {
               helios.tcp {
                          transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                          transport-protocol = tcp
                          port = " + akkaConf.Network.PortNumber + @"
               }
               hostname = " + akkaConf.Network.Name + @"
        }";
        return remoteConfig;
    }

    public string Build()
    {
        return BuildDeployConfig(_akkaConf);
    }
}