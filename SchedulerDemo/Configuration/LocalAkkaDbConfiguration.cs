using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace SchedulerDemo.Configuration
{
    public class LocalAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString
            => "Data Source=(local);Initial Catalog=AutoTestAkka;Integrated Security = true";

        public string JournalConnectionString
            => "Data Source=(local);Initial Catalog=AutoTestAkka;Integrated Security = true";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public string SnapshotTableName => "Snapshots";
    }
}