namespace GridDomain.Node.Configuration {
    public static class ActorSystemBuilderExtensions
    {
        public static ActorSystemConfigBuilder LocalInMemory(this ActorSystemConfigBuilder configBuilder,bool serializeMessagesCreators = false)
        {
            return configBuilder
                   .DomainSerialization(serializeMessagesCreators)
                   .RemoteActorProvider()
                   .InMemoryPersistence();
        }
    }
}