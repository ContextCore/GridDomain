﻿using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;

namespace GridDomain.Tests.Acceptance
{
    public static class TestEnvironment
    {
        public static readonly IDbConfiguration Configuration
            = new AutoTestLocalDbConfiguration();

        public static readonly IAkkaDbConfiguration AkkaConfiguration
            = new AutoTestAkkaDbConfiguration();
    }

    public class AutoTestAkkaDbConfiguration : IAkkaDbConfiguration
    {
        public string SnapshotConnectionString =>
                @"Data Source=(localdb)\\v11.0;Database=AutoTestAkka;Integrated Security = true"; 
        public string JournalConnectionString =>
              @"Data Source=(localdb)\\v11.0;Database=AutoTestAkka;Integrated Security = true";

        public string MetadataTableName => "Metadata";
        public string JournalTableName => "Journal";
        public string SnapshotTableName => "Snapshots";
    }
}