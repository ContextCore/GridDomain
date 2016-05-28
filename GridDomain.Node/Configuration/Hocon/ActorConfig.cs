using System;
using System.Linq;
using Akka.Actor;
using GridDomain.Node.Configuration;


internal class ActorConfig: IAkkaConfig
{
    private readonly string[] _seedNodes;
    private readonly int _port;
    private readonly string _name;

    private ActorConfig(int port, string name, params string[] seedNodes)
    {
        _name = name;
        _port = port;
        _seedNodes = seedNodes;
    }

    public static ActorConfig ClusterSeedNode(IAkkaNetworkAddress address, params IAkkaNetworkAddress[] otherSeeds)
    {
        var allSeeds = otherSeeds.Union(new []{ address});
        var seedNodes = allSeeds.Select(GetSeedNetworkAddress).ToArray();
        return new ActorConfig(address.PortNumber,address.Name, seedNodes);
    }

    public static ActorConfig ClusterNonSeedNode(string name, IAkkaNetworkAddress[] seedNodesAddresses)
    {
        var seedNodes = seedNodesAddresses.Select(GetSeedNetworkAddress).ToArray();
        return new ActorConfig(0, name, seedNodes);
    }

    public string Build()
    {
        string actorConfig = @"   
       actor {
             serializers {
                         wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""
             }

             serialization-bindings {
                                    ""System.Object"" = wire
             }
             
             loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
             debug {
                   receive = on
                   autoreceive = on
                   lifecycle = on
                   event-stream = on
                   unhandled = on
             }

       }";

        var deploy = BuildClusterNode(_port,_name, _seedNodes);

        return actorConfig + Environment.NewLine + deploy;
    }

    private string BuildClusterNode( int portNumber, string name, params string[] seedNodes)
    {
        string seeds = string.Join(Environment.NewLine, seedNodes.Select(n => @"""" + n + @""""));

        string clusterConfigString = 
            @"
            actor.provider = ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
            cluster {
                            seed-nodes = [" + seeds + @"]
            }
            cluster.sharding.journal-plugin-id = ""akka.persistence.journal.sql-server""
            cluster.sharding.snapshot-plugin-id = ""akka.persistence.snapshot-store.sql-server""
          
            remote {
                    helios.tcp {
                                  transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                  transport-protocol = tcp
                                  port = " + portNumber + @"
                    }
                    hostname = " + name + @"
            }";
        return clusterConfigString;
    }

    private static string GetSeedNetworkAddress(IAkkaNetworkAddress conf)
    {
        string networkAddress = $"akka.tcp://{conf.Name}@{conf.Host}:{conf.PortNumber}";
        return networkAddress;
    }
}