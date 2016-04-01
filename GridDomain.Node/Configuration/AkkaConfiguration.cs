namespace GridDomain.Node.Configuration
{
    public class AkkaConfiguration
    {
        public int Port { get; }
        public string Name { get; }
        public string Host { get; }
        public AkkaConfiguration(string name, int portNumber, string host)
        {
            Name = name;
            Port = portNumber;
            Host = host;
        }
    }
}