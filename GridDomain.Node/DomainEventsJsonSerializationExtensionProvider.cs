using Akka.Actor;

namespace GridDomain.Node
{
    public class DomainEventsJsonSerializationExtensionProvider : ExtensionIdProvider<DomainEventsJsonSerializationExtension>
    {
        /// <summary>
        /// A static reference to the current provider.
        /// </summary>
        public static readonly DomainEventsJsonSerializationExtensionProvider Provider = new DomainEventsJsonSerializationExtensionProvider();

        /// <summary>
        /// Creates the dependency injection extension using a given actor system.
        /// </summary>
        /// <param name="system">The actor system to use when creating the extension.</param>
        /// <returns>The extension created using the given actor system.</returns>
        public override DomainEventsJsonSerializationExtension CreateExtension(ExtendedActorSystem system)
        {
            return new DomainEventsJsonSerializationExtension();
        }
    }
}