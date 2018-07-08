using System;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Unit.CommandsExecution;

namespace GridDomain.Tests.Acceptance
{
    public class AutoTestLocalDbConfiguration : IDbConfiguration
    {
        private const string JournalConnectionStringName = "ReadModel";
        
        public string ReadModelConnectionString
            => Environment.GetEnvironmentVariable(JournalConnectionStringName) ??  @"Data Source=localhost,1400;Initial Catalog=AutoTestRead;User = sa; Password = P@ssw0rd1;";

        public string LogsConnectionString
            => Environment.GetEnvironmentVariable(JournalConnectionStringName) ?? @"Data Source=localhost,1400;Initial Catalog=AutoTestLogs;User = sa; Password = P@ssw0rd1;";
    }
}