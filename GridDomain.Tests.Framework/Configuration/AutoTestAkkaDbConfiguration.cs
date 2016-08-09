using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString
            => "Data Source=(local);Database=AutoTestAkka;Integrated Security = true";

        public string JournalConnectionString
            => "Data Source=(local);Database=AutoTestAkka;Integrated Security = true";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "JournalEntry";
        public string SnapshotTableName => "Snapshots";
    }
}