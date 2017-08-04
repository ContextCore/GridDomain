using System;
using System.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Common.Configuration
{

    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        private const string JournalConnectionStringName = "WriteModel";
        //enviroment variables - for appveour tests launch
        public string SnapshotConnectionString
            =>
                ConfigurationManager.ConnectionStrings[JournalConnectionStringName]?.ConnectionString
                ?? Environment.GetEnvironmentVariable(JournalConnectionStringName) ?? "Server=(local); Database = AutoTestWrite; Integrated Security = true; MultipleActiveResultSets = True";

        public string JournalConnectionString
            =>
                ConfigurationManager.ConnectionStrings[JournalConnectionStringName]?.ConnectionString
                ?? Environment.GetEnvironmentVariable(JournalConnectionStringName) ?? "Server=(local); Database = AutoTestWrite; Integrated Security = true; MultipleActiveResultSets = True";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public int JornalConnectionTimeoutSeconds => 120;
        public int SnapshotsConnectionTimeoutSeconds => 120;
        public string SnapshotTableName => "Snapshots";
        public string SchemaName { get; } = "dbo";
    }
}