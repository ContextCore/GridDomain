namespace GridDomain.Node.Configuration
{
    public class AkkaConfiguration
    {
        public AkkaConfiguration(string name, int portNumber, string host)
        {
            Name = name;
            Port = portNumber;
            Host = host;
        }

        public int Port { get; }
        public string Name { get; }
        public string Host { get; }
    }
}