namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class PersistenceConfig : IAkkaConfig
    {
        private readonly AkkaConfiguration _akka;

        public PersistenceConfig(AkkaConfiguration akka)
        {
            _akka = akka;
        }

        public string Build()
        {
            var akkaPersistenceConfig =
        @"persistence {
                    publish-plugin-commands = on
" + new PersistenceJournalConfig(_akka.Persistence, new DomainEventAdaptersConfig()).Build() + @"
" + new PersistenceSnapshotConfig(_akka).Build() + @"
        }";
            return akkaPersistenceConfig;
        }
    }
}