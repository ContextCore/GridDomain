using System.Configuration;

namespace GridDomain.Node.Configuration.Akka {

    public class WriteDbConfigSection : ConfigurationSection, IAkkaDbConfiguration
    {
        [ConfigurationProperty(nameof(SnapshotConnectionString))]
        public string SnapshotConnectionString => (string)base[nameof(SnapshotConnectionString)];

        [ConfigurationProperty(nameof(JournalConnectionString))]
        public string JournalConnectionString => (string)base[nameof(JournalConnectionString)];

        [ConfigurationProperty(nameof(MetadataTableName),DefaultValue = "Metadata",IsRequired = false)]
        public string MetadataTableName => (string)base[nameof(MetadataTableName)];

        [ConfigurationProperty(nameof(JournalTableName), DefaultValue = "Journal", IsRequired = false)]
        public string JournalTableName => (string)base[nameof(JournalTableName)];

        [ConfigurationProperty(nameof(JornalConnectionTimeoutSeconds), DefaultValue = 30, IsRequired = false)]
        public int JornalConnectionTimeoutSeconds => (int)base[nameof(JornalConnectionTimeoutSeconds)];

        [ConfigurationProperty(nameof(SnapshotsConnectionTimeoutSeconds), DefaultValue =30, IsRequired = false)]
        public int SnapshotsConnectionTimeoutSeconds => (int)base[nameof(SnapshotsConnectionTimeoutSeconds)];

        [ConfigurationProperty(nameof(SnapshotTableName), DefaultValue = "SnapShots", IsRequired = false)]
        public string SnapshotTableName => (string)base[nameof(SnapshotTableName)];

        [ConfigurationProperty(nameof(SchemaName), DefaultValue = "dbo", IsRequired = false)]
        public string SchemaName => (string)base[nameof(SchemaName)];
    }
}