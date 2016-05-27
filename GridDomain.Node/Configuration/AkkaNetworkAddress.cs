namespace GridDomain.Node.Configuration
{
    public class AkkaNetworkAddress : IAkkaNetworkAddress
    {
        public string Name { get;}
        public string Host { get;}
        public int PortNumber { get; }

        public AkkaNetworkAddress(string name, string host, int port )
        {
            Name = name;
            Host = host;
            PortNumber = port;
        }

        public override bool Equals(object obj)
        {
            return Equals((AkkaNetworkAddress)obj);
        }

        protected bool Equals(AkkaNetworkAddress other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Host, other.Host) && PortNumber == other.PortNumber;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Host != null ? Host.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ PortNumber;
                return hashCode;
            }
        }
    }
}