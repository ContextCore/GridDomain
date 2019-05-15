using System;
using System.Linq;
using GridDomain.Node.Akka.Configuration.Hocon;

namespace GridDomain.Node.Akka.Cluster.Hocon
{
    public class ClusterSeedNodes : IHoconConfig
    {
        private readonly string[] _seedNodeFullAddresses;

        public ClusterSeedNodes(params NodeNetworkAddress[] addresses) : this(
            addresses.Select(a => a.ToTcpAddress()).ToArray())
        {
        }

        public ClusterSeedNodes(params string[] seedNodeFullAddresses)
        {
            _seedNodeFullAddresses = seedNodeFullAddresses;
        }

        public string Build()
        {
            var seeds = string.Join(Environment.NewLine, _seedNodeFullAddresses.Select(n => @"""" + n + @""""));

            if (seeds.Any())
            {
                return @"
                akka.cluster {
                    seed-nodes =  ["
                       + seeds
                       + @"]
                }
                ";
            }

            return "";
        }
    }
}