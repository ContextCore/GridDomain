using GridDomain.Node.Configuration;

internal class PersistenceConfig : IAkkaConfig
{
    private AkkaConfiguration _akka;

    public PersistenceConfig(AkkaConfiguration akka)
    {
        _akka = akka;
    }

    public string Build()
    {
        string akkaPersistenceConfig =
            @"      persistence {
                    publish-plugin-commands = on
" + new PersistenceJournalConfig(_akka).Build() + @"
" + new PersistenceSnapshotConfig(_akka).Build() + @"
        }";
        return akkaPersistenceConfig;
    }
}