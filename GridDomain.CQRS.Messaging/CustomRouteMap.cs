using System;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public class CustomRouteMap : IMessageRouteMap
    {
        private readonly Action<IMessagesRouter>[] _routeRules;

        public CustomRouteMap(params Action<IMessagesRouter>[] routeRules)
        {
            _routeRules = routeRules;
        }

        public void Register(IMessagesRouter router)
        {
            foreach (var routeRule in _routeRules)
            {
                routeRule(router);
            }
        }
    }
}