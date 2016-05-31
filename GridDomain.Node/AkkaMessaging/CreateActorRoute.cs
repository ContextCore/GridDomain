using System;
using CommonDomain.Core;

namespace GridDomain.Node.AkkaMessaging
{

    public class MessageRoute
    {
        public Type MessageType { get; set; }
        public string CorrelationField { get; set; }
    }

    public class CreateActorRoute
    {
        public MessageRoute[] Routes { get; }

        public Type AggregateType { get; }
        
        public CreateActorRoute(Type aggregateType, params MessageRoute[] routes)
        {
            Routes = routes;
            AggregateType = aggregateType;
        }

    }
}