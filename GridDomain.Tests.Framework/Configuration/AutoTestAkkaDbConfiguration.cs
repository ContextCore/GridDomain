using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString
            => "Data Source=(localdb)\\\\v11.0;Database=AutoTestAkka;Integrated Security = true";

        public string JournalConnectionString
            => "Data Source=(localdb)\\\\v11.0;Database=AutoTestAkka;Integrated Security = true";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public string SnapshotTableName => "Snapshots";
    }
}