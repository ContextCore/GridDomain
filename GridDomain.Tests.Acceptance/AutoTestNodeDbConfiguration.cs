using System;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Tests.Acceptance
{

    public class AutoTestNodeDbConfiguration : DefaultNodeDbConfiguration
    {
        public static ISqlNodeDbConfiguration Default { get; } = new AutoTestNodeDbConfiguration();

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