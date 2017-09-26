using GridDomain.Node.Configuration;

namespace GridDomain.Tools.Connector
{
    internal class ConsoleDbConfig : INodeDbConfiguration
    {
        public string SnapshotConnectionString { get; }
        public string JournalConnectionString { get; }
        public string MetadataTableName { get; }
        public string JournalTableName { get; }
        public int JornalConnectionTimeoutSeconds { get; } = 30;
        public int SnapshotsConnectionTimeoutSeconds { get; } = 30;
        public string SnapshotTableName { get; }
        public string SchemaName { get; } = "dbo";
    }
}