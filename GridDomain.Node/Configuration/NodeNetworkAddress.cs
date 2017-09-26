namespace GridDomain.Node.Configuration
{
    public class NodeNetworkAddress : INodeNetworkAddress
    {
        public NodeNetworkAddress(string systemName,
                                  string host,
                                  int port,
                                  string publicHost = null,
                                  bool enforceIpVersion = true)
        {
            SystemName = systemName;
            Host = host;
            PortNumber = port;
            PublicHost = publicHost ?? Host;
            EnforceIpVersion = enforceIpVersion;
        }

        public string SystemName { get; }
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
            return string.Equals(SystemName, other.SystemName) && string.Equals(Host, other.Host)
                   && PortNumber == other.PortNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SystemName != null ? SystemName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (Host != null ? Host.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PortNumber;
                return hashCode;
            }
        }
    }
}