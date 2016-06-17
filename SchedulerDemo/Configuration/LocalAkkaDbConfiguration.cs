using GridDomain.Node.Configuration;

namespace SchedulerDemo.Configuration
{
    public class LocalAkkaDbConfiguration : IAkkaDbConfiguration
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