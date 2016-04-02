namespace GridDomain.Node.Configuration
{
    public class AkkaConfiguration
    {
        public int Port { get; }
        public string Name { get; }
        public string Host { get; }

        public string LogLevel { get; }
        public AkkaConfiguration(string name, int portNumber, string host, string logLevel="Info")
        {
            Name = name;
            Port = portNumber;
            Host = host;
            LogLevel = logLevel;
        }
    }
}