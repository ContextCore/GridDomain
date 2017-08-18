using System;
using GridDomain.Node.Configuration.Persistence;

namespace GridDomain.Tests.Common
{
    public class AutoTestLocalDbConfiguration : IDbConfiguration
    {
        private const string JournalConnectionStringName = "ReadModel";
        
        public string ReadModelConnectionString
            => Environment.GetEnvironmentVariable(JournalConnectionStringName) ??  @"Data Source=(local);Initial Catalog=AutoTestRead;Integrated Security = true";

        public string LogsConnectionString
            => Environment.GetEnvironmentVariable(JournalConnectionStringName) ?? @"Data Source=(local);Initial Catalog=AutoTestLogs;Integrated Security = true";
    }
}