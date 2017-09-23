using System;
using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tests.Common.Configuration
{

    public class AutoTestAkkaDbConfiguration : DefaultAkkaDbConfiguration
    {
        public AutoTestAkkaDbConfiguration():base("Server=(local); Database = AutoTestWrite; Integrated Security = true; MultipleActiveResultSets = True")
        {
            
        }
        private const string JournalConnectionStringName = "WriteModel";
        //enviroment variables - for appveour tests launch
        public override string SnapshotConnectionString
            => Environment.GetEnvironmentVariable(JournalConnectionStringName) ?? base.SnapshotConnectionString;

        public override string JournalConnectionString
            => Environment.GetEnvironmentVariable(JournalConnectionStringName) ?? base.SnapshotConnectionString;
        }
}