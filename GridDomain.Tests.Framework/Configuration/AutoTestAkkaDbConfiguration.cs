using System.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        private const string JournalConnectionStringName = "WriteModel";

        public string SnapshotConnectionString
            => ConfigurationManager.ConnectionStrings[JournalConnectionStringName]?.ConnectionString ??
            "Server=(local); Database = AutoTestAkka; Integrated Security = true; MultipleActiveResultSets = True";

        public string JournalConnectionString
            => ConfigurationManager.ConnectionStrings[JournalConnectionStringName]?.ConnectionString ??
            "Server=(local); Database = AutoTestAkka; Integrated Security = true; MultipleActiveResultSets = True";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public int JornalConnectionTimeoutSeconds => 120;
        public int SnapshotsConnectionTimeoutSeconds => 120;
        public string SnapshotTableName => "Snapshots";
    }
}