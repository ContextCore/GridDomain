namespace GridDomain.Node
{
    public class NodeNetworkAddress 
    {
        public NodeNetworkAddress(string host,
                                  int port,
                                  string publicHost,
                                  string name)
        {
            Host = host ?? "localhost";
            PortNumber = port;
            PublicHost = publicHost ?? Host;
            Name = name;
        }

        public string Host { get; }
        public string PublicHost { get; }
        public int PortNumber { get; }
        public string Name { get; }

        public override bool Equals(object obj)
        {
            return Equals((NodeNetworkAddress) obj);
        }

        protected bool Equals(NodeNetworkAddress other)
        {
            return  string.Equals(Host, other.Host) && PortNumber == other.PortNumber;
        }

        public NodeNetworkAddress Copy(int? newPort = null)
        {
            return new NodeNetworkAddress(Host,newPort ?? PortNumber,PublicHost,Name);
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