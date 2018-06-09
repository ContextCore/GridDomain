namespace GridDomain.Node.Configuration {
    public static class ActorSystemBuilderExtensions
    {
        public static ActorSystemBuilder LocalInMemory(this ActorSystemBuilder builder,bool serializeMessagesCreators = false)
        {
            return builder
                   .DomainSerialization(serializeMessagesCreators)
                   .RemoteActorProvider()
                   .InMemoryPersistence();
        }
    }
}