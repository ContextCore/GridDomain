using GridDomain.Node.Configuration.Persistence;

namespace GridDomain.Tests.Framework
{
    public class AutoTestLocalDbConfiguration : IDbConfiguration
    {
        public string ReadModelConnectionString
            => @"Data Source=(local);Initial Catalog=AutoTestGridDomainRead;Integrated Security = true";

        public string LogsConnectionString
            => @"Data Source=(local);Initial Catalog=AutoTestGridDomainLogs;Integrated Security = true";
    }
}