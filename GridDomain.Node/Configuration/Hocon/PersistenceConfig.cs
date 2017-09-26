namespace GridDomain.Node.Configuration.Hocon
{
    internal class PersistenceConfig : INodeConfig
    {
        private readonly INodeConfig _journalConfig;
        private readonly INodeConfig _snapshotsConfig;

        public PersistenceConfig(INodeConfig journalConfig, INodeConfig snapshotsConfig)
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