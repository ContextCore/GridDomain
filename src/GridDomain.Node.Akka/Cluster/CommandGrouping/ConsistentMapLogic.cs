using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Node.Akka.Cluster.CommandGrouping
{
    class ConsistentMapLogic : RoutingLogic
    {
        private readonly IDictionary<string, Routee> _keyedPaths;
        private readonly ConsistentMapping _consistentMapping;
        private readonly ActorSystem _system;

        public ConsistentMapLogic(ActorSystem system, IDictionary<string,Routee> keyedPaths, ConsistentMapping mapping=null)
        {
            _system = system;
            _consistentMapping = mapping ?? DefaultMap;
            _keyedPaths = keyedPaths;
            if(!keyedPaths.Any())
                system.Log.Warning("No available routees, may be we miss some aggregates registration?");

        }
        
        
        public override Routee Select(object message, Routee[] routees)
        {
            var pathKey = _consistentMapping(message);
            if (_keyedPaths.TryGetValue(pathKey, out Routee route)) return route;
            _system.Log.Warning("Could not find requested route: " + pathKey);
            return Routee.NoRoutee;
        }

        private string DefaultMap(object msg)
        {
            switch (msg)
            {
//                case IMessageMetadataEnvelop env:
//                    return DefaultMap(env.Message);
                case ICommand cmd:
                    return cmd.Recipient.Name;
                default:
                    throw new InvalidMessageException(msg.ToString());
            }
        }
    }
}