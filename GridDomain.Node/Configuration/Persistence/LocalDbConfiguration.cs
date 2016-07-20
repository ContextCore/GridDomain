namespace GridDomain.Node.Configuration.Persistence
{
    public class LocalDbConfiguration : IDbConfiguration
    {
        public string ReadModelConnectionString
            => @"Data Source=(local);Initial Catalog=AutoTestGridDomainRead;Integrated Security = true";

        public string LogsConnectionString
            => @"Data Source=(local);Initial Catalog=AutoTestGridDomainLogs;Integrated Security = true";
    }
}