using Akka.Actor;

namespace GridDomain.Node.Serializers
{
    public class DomainEventsJsonSerializationExtensionProvider : ExtensionIdProvider<DomainEventsJsonSerializationExtension>
    {
        /// <summary>
        ///     A static reference to the current provider.
        /// </summary>
        public static readonly DomainEventsJsonSerializationExtensionProvider Provider =
            new DomainEventsJsonSerializationExtensionProvider();

        public override DomainEventsJsonSerializationExtension CreateExtension(ExtendedActorSystem system)
        {
            return new DomainEventsJsonSerializationExtension();
        }
    }
}