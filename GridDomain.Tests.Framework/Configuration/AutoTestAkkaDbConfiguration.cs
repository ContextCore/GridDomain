using System.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        private const string JournalConnectionStringName = "WriteModel";

        public string SnapshotConnectionString
            => ConfigurationManager.ConnectionStrings[JournalConnectionStringName]?.ConnectionString ?? "Server=tcp:soloinfra.cloudapp.net,5099;Database=sandboxMembershipWriteAutoTests;User ID=solomoto;Password=s0l0moto;MultipleActiveResultSets=True";

        public string JournalConnectionString
            => ConfigurationManager.ConnectionStrings[JournalConnectionStringName]?.ConnectionString ?? "Server=tcp:soloinfra.cloudapp.net,5099;Database=sandboxMembershipWriteAutoTests;User ID=solomoto;Password=s0l0moto;MultipleActiveResultSets=True";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public string SnapshotTableName => "Snapshots";
    }
}