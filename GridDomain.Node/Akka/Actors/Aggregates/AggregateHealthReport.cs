using System;

namespace GridDomain.Node.Akka.Actors.Aggregates
{
    public class AggregateHealthReport
    {
        public string Path { get; }
        public string NodeAddress { get; }

        public TimeSpan Uptime { get; }

        public AggregateHealthReport(string path, TimeSpan uptime, string nodeAddress)
        {
            Path = path;
            Uptime = uptime;
            NodeAddress = nodeAddress;
        }
    }
}