using System;
using System.Collections.Generic;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Tasks;

namespace GridDomain.Scheduling.Akka
{
    public class TaskRouter : ITaskRouter
    {
        private static Dictionary<Type, IActorRef> _routeTable;

        public TaskRouter(Dictionary<Type, IActorRef> routeTable = null)
        {
            _routeTable = routeTable ?? new Dictionary<Type, IActorRef>();
        }

        public void AddRoute(Type requestType, IActorRef target)
        {
            _routeTable[requestType] = target;
        }

        public IActorRef GetTarget(ProcessScheduledTaskRequest request)
        {
            return _routeTable[request.GetType()];
        }
    }
}