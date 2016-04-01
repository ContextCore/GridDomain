namespace GridDomain.Node.Configuration
{
    internal class LocalDbConfiguration : IDbConfiguration
    {
        public string EventStoreConnectionString
            => @"Data Source=(localdb)\v11.0;Initial Catalog=AutoTestGridDomainEvent;Integrated Security = true";

        public string ReadModelConnectionString
            => @"Data Source=(localdb)\v11.0;Initial Catalog=AutoTestGridDomainRead;Integrated Security = true";

        public string LogsConnectionString
            => @"Data Source=(localdb)\v11.0;Initial Catalog=AutoTestGridDomainLogs;Integrated Security = true";
    }
}