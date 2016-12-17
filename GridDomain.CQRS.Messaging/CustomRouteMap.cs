using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public class CustomRouteMap : IMessageRouteMap
    {
        private readonly List<Action<IMessagesRouter>> _routeRules = new List<Action<IMessagesRouter>>();

        public CustomRouteMap(params Action<IMessagesRouter>[] routeRules)
        {
            _routeRules = routeRules.ToList();
        }
        public CustomRouteMap(IMessageRouteMap baseMap, params Action<IMessagesRouter>[] routeRules)
        {
            _routeRules.Add( baseMap.Register);
            _routeRules.AddRange(routeRules);
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