using GridDomain.Node.Configuration;

internal class BuildPersistenceConfig : IAkkaConfig
{
    private AkkaConfiguration _akka;

    public BuildPersistenceConfig(AkkaConfiguration akka)
    {
        _akka = akka;
    }

    public string BuildAkkaPersistenceConfig()
    {
        string akkaPersistenceConfig =
            @"      persistence {
                    publish-plugin-commands = on
" + new PersistenceJournalConfig(_akka).Build() + @"
" + new PersistenceJournalConfig(_akka).Build() + @"
        }";
        return akkaPersistenceConfig;
    }

    public string Build()
    {
        return BuildAkkaPersistenceConfig();
    }
}