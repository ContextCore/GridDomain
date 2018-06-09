using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon
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

        public Config Build()
        {
            var akkaPersistenceConfig = @"persistence{
                    publish-plugin-commands = on
" + _journalConfig.Build().ToString() + @"
" + _snapshotsConfig.Build().ToString() + @"
        }";
            return akkaPersistenceConfig;
        }
    }
}