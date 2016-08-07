using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Console
{
    class ConsoleDbConfig : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString { get; }
        public string JournalConnectionString { get; }
        public string MetadataTableName { get; }
        public string JournalTableName { get; }
        public string SnapshotTableName { get; }
    }
}