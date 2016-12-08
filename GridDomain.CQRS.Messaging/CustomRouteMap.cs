using System;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.CQRS.Messaging
{
    public class CustomRouteMap : IMessageRouteMap
    {
        private readonly Func<IMessagesRouter, Task>[] _routeRules;

        public CustomRouteMap(params Func<IMessagesRouter, Task>[] routeRules)
        {
            _routeRules = routeRules;
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