using System.Configuration;

namespace GridDomain.Node.Configuration.Persistence
{
    public class LocalDbConfiguration : IDbConfiguration
    {
        private const string LogsConnectionStringName = "Logs";
        private const string ReadModelConnectionStringName = "ReadModel";

        public string ReadModelConnectionString
            =>
                ConfigurationManager.ConnectionStrings[ReadModelConnectionStringName]?.ConnectionString
                ?? @"Server=tcp:soloinfra.cloudapp.net,5099;Database=sandboxMembershipReadAutoTests;User ID=solomoto;Password=s0l0moto;MultipleActiveResultSets=True"
        ;

        public string LogsConnectionString
            =>
                ConfigurationManager.ConnectionStrings[LogsConnectionStringName]?.ConnectionString
                ?? @"Server=tcp:soloinfra.cloudapp.net,5099;Database=sandboxMembershipLogsAutoTests;User ID=solomoto;Password=s0l0moto;MultipleActiveResultSets=True"
        ;
    }
}