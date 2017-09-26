using System;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Common.Configuration
{

    public class AutoTestNodeDbConfiguration : DefaultNodeDbConfiguration
    {
        public AutoTestNodeDbConfiguration():base("Server=(local); Database = AutoTestWrite; Integrated Security = true; MultipleActiveResultSets = True")
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