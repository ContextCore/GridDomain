namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class PersistenceConfig : IAkkaConfig
    {
        private readonly IAkkaConfig _journalConfig;
        private readonly IAkkaConfig _snapshotsConfig;

        public PersistenceConfig(IAkkaConfig journalConfig, IAkkaConfig snapshotsConfig)
        {
            _snapshotsConfig = snapshotsConfig;
            _journalConfig = journalConfig;
        }

        public string Build()
        {
            var akkaPersistenceConfig = @"persistence {
                    publish-plugin-commands = on
" + _journalConfig.Build() + @"
" + _snapshotsConfig.Build() + @"
        }";
            return akkaPersistenceConfig;
        }
    }
}