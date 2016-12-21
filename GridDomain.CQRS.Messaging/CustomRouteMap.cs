using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public class CustomRouteMap : IMessageRouteMap
    {
        private readonly List<Func<IMessagesRouter, Task>> _routeRules = new List<Func<IMessagesRouter, Task>>();

        public CustomRouteMap(params Func<IMessagesRouter, Task>[] routeRules)
        {
            _routeRules = routeRules.ToList();
        }
        public CustomRouteMap(IMessageRouteMap baseMap, params Func<IMessagesRouter, Task>[] routeRules)
        {
            _routeRules.Add( baseMap.Register);
            _routeRules.AddRange(routeRules);
        }


        public async Task Register(IMessagesRouter router)
        {
            foreach (var routeRule in _routeRules)
            {
               await routeRule(router);
            }
        }
    }
}