using System;
using System.Configuration;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Common.Configuration
{

    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        private const string JournalConnectionStringName = "WriteModel";
        private const string AppVeyorEnabled = "APPVEYOR";

        //enviroment variables - for appveour tests launch
        public string SnapshotConnectionString
            =>
                ConfigurationManager.ConnectionStrings[JournalConnectionStringName]?.ConnectionString
                ?? GetFromAppveyourEnvironment() ?? "Server=(local); Database = AutoTestWrite; Integrated Security = true; MultipleActiveResultSets = True";

        public string JournalConnectionString
            =>
                ConfigurationManager.ConnectionStrings[JournalConnectionStringName]?.ConnectionString
                ?? GetFromAppveyourEnvironment() ?? "Server=(local); Database = AutoTestWrite; Integrated Security = true; MultipleActiveResultSets = True";

        private static string GetFromAppveyourEnvironment()
        {
            return string.IsNullOrEmpty(Environment.GetEnvironmentVariable(AppVeyorEnabled)) ? null :
                "(local)\\SQL2016; Database = master; User ID = sa; Password = Password12!";
        }

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public int JornalConnectionTimeoutSeconds => 120;
        public int SnapshotsConnectionTimeoutSeconds => 120;
        public string SnapshotTableName => "Snapshots";
        public string SchemaName { get; } = "dbo";
    }
}