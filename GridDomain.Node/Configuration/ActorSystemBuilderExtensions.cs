namespace GridDomain.Node.Configuration {
    public static class ActorSystemBuilderExtensions
    {
        public static IActorSystemConfigBuilder LocalInMemory(this IActorSystemConfigBuilder configBuilder,bool serializeMessagesCreators = false)
        {
            return configBuilder
                   .DomainSerialization(serializeMessagesCreators)
                   .RemoteActorProvider()
                   .InMemoryPersistence();
        }
    }
}