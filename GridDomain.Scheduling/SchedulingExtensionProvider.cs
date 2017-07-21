using Akka.Actor;

namespace GridDomain.Scheduling {
    public class SchedulingExtensionProvider : ExtensionIdProvider<SchedulingExtension>
    {
        /// <summary>
        ///     A static reference to the current provider.
        /// </summary>
        public static readonly SchedulingExtensionProvider Provider =
            new SchedulingExtensionProvider();

        public override SchedulingExtension CreateExtension(ExtendedActorSystem system)
        {
            return new SchedulingExtension();
        }
    }
}