namespace GridDomain.Node.Akka.Configuration.Hocon
{
    public class PersistenceConfig : IHoconConfig
    {
        private readonly IHoconConfig _journalConfig;
        private readonly IHoconConfig _snapshotsConfig;

        public PersistenceConfig(IHoconConfig journalConfig, IHoconConfig snapshotsConfig)
        {
            _snapshotsConfig = snapshotsConfig;
            _journalConfig = journalConfig;
        }

        public string Build()
        {
            var akkaPersistenceConfig = @"akka.persistence.publish-plugin-commands = on
"
                                        + _journalConfig.Build()
                                        + @"
"
                                        + _snapshotsConfig.Build();
            return akkaPersistenceConfig;
        }
    }
}