using Akka.Actor;
using GridDomain.Node.Configuration.Hocon;

namespace GridDomain.Node.Configuration {
    public static class NodeConfigurationExtensions
    {
        public static ActorSystem CreateInMemorySystem(this NodeConfiguration conf)
        {
            return ActorSystem.Create(conf.Name, conf.ToStandAloneInMemorySystemConfig());
        }

        public static string ToStandAloneInMemorySystemConfig(this NodeConfiguration conf,bool serializeMessagesCreators = false)
        {

            return ActorSystemBuilder.New()
                                     .Log(conf.LogLevel)
                                     .DomainSerialization(serializeMessagesCreators)
                                     .RemoteActorProvider()
                                     .Remote(conf.Address)
                                     .InMemoryPersistence()
                                     .BuildHocon();
        }

    }
}