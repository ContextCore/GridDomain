using System;

namespace GridDomain.Node.AkkaMessaging
{
    public class CreateActorRoute
    {
        public CreateActorRoute(Type aggregateType, params MessageRoute[] routes)
        {
            Routes = routes;
            AggregateType = aggregateType;
        }

        public MessageRoute[] Routes { get; }

        public Type AggregateType { get; }
    }
}