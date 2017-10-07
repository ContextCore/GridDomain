namespace GridDomain.Node.Configuration
{
    public class NodeNetworkAddress : INodeNetworkAddress
    {
        public NodeNetworkAddress(string host=null,
                                  int port =0,
                                  string publicHost = null,
                                  bool enforceIpVersion = true)
        {
            Host = host ?? "localhost";
            PortNumber = port;
            PublicHost = publicHost ?? Host;
            EnforceIpVersion = enforceIpVersion;
        }

        public string Host { get; }
        public string PublicHost { get; }
        public int PortNumber { get; }
        public bool EnforceIpVersion { get; }

        public override bool Equals(object obj)
        {
            return Equals((NodeNetworkAddress) obj);
        }

        protected bool Equals(NodeNetworkAddress other)
        {
            return  string.Equals(Host, other.Host) && PortNumber == other.PortNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Host != null ? Host.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PortNumber;
                return hashCode;
            }
        }
    }
}