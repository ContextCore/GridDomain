using System.Configuration;
using GridDomain.Node.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Framework.Configuration
{
    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString
            => ConfigurationManager.ConnectionStrings["GridDomainWriteTestString"].ConnectionString ?? "Server=tcp:soloinfra.cloudapp.net,5099;Database=sandboxMembershipWriteAutoTests;User ID=solomoto;Password=s0l0moto;MultipleActiveResultSets=True";

        public string JournalConnectionString
            => ConfigurationManager.ConnectionStrings["GridDomainWriteTestString"].ConnectionString ?? "Server=tcp:soloinfra.cloudapp.net,5099;Database=sandboxMembershipWriteAutoTests;User ID=solomoto;Password=s0l0moto;MultipleActiveResultSets=True";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public string SnapshotTableName => "Snapshots";
    }
}