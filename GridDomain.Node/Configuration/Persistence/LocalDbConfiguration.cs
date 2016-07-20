namespace GridDomain.Node.Configuration.Persistence
{
    public class LocalDbConfiguration : IDbConfiguration
    {
        public string ReadModelConnectionString
            => @"Data Source=(localdb)\v11.0;Initial Catalog=AutoTestGridDomainRead;Integrated Security = true";

        public string LogsConnectionString
            => @"Data Source=(localdb)\v11.0;Initial Catalog=AutoTestGridDomainLogs;Integrated Security = true";
    }
}