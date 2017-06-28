using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public CustomRouteMap(string name, params Func<IMessagesRouter, Task>[] routeRules):this(routeRules)
        {
            Name = name;
        }

        public async Task Register(IMessagesRouter router)
        {
            foreach (var routeRule in _routeRules)
                await routeRule(router);
        }

        public string Name { get; }
    }
}