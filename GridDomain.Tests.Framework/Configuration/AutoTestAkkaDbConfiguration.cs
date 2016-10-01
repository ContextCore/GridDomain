using System.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString
            => ConfigurationManager.ConnectionStrings["GridDomainWriteTestString"]?.ConnectionString ?? "Server=(local);Database=AutoTestAkka;Integrated Security = true;";

        public string JournalConnectionString
            => ConfigurationManager.ConnectionStrings["GridDomainWriteTestString"]?.ConnectionString ?? "Server=(local);Database=AutoTestAkka;Integrated Security = true;";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public string SnapshotTableName => "Snapshots";
    }
}