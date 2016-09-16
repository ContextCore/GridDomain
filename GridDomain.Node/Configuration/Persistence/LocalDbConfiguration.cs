namespace GridDomain.Node.Configuration.Persistence
{
    public class LocalDbConfiguration : IDbConfiguration
    {
        public string ReadModelConnectionString
            => @"Server=tcp:soloinfra.cloudapp.net,5099;Database=sandboxMembershipReadAutoTests;User ID=solomoto;Password=s0l0moto;MultipleActiveResultSets=True";

        public string LogsConnectionString
            => @"Server=tcp:soloinfra.cloudapp.net,5099;Database=sandboxMembershipLogsAutoTests;User ID=solomoto;Password=s0l0moto;MultipleActiveResultSets=True";
    }
}