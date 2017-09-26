namespace GridDomain.Node.Configuration {
    public class DefaultNodeDbConfiguration : INodeDbConfiguration
    {
        private readonly string _writeDbConnectionString;

        public DefaultNodeDbConfiguration(string connectionString)
        {
            _writeDbConnectionString = connectionString;
        }
        //enviroment variables - for appveour tests launch
        public virtual string SnapshotConnectionString => _writeDbConnectionString;

        public virtual string JournalConnectionString => _writeDbConnectionString;

        public virtual string MetadataTableName => "Metadata";
        public virtual string JournalTableName => "Journal";
        public virtual int JornalConnectionTimeoutSeconds => 120;
        public virtual int SnapshotsConnectionTimeoutSeconds => 120;
        public virtual string SnapshotTableName => "Snapshots";
        public virtual string SchemaName { get; } = "dbo";
    }
}