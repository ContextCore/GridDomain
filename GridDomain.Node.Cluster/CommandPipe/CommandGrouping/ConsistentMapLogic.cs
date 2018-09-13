using System.Collections.Generic;
using Akka.Actor;
using Akka.Routing;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Cluster.CommandPipe.CommandGrouping
{
    class ConsistentMapLogic : RoutingLogic
    {
        private readonly IDictionary<string, Routee> _keyedPaths;
        private readonly ConsistentMapping _consistentMapping;

        public ConsistentMapLogic(ActorSystem system, IDictionary<string,Routee> keyedPaths, ConsistentMapping mapping=null)
        {
            _consistentMapping = mapping ?? DefaultMap;
            _keyedPaths = keyedPaths;
        }
        
        
        public override Routee Select(object message, Routee[] routees)
        {
           return _keyedPaths[_consistentMapping(message)];
        }

        private string DefaultMap(object msg)
        {
            switch (msg)
            {
                case IMessageMetadataEnvelop env:
                    return DefaultMap(env.Message);
                case ICommand cmd:
                    return cmd.AggregateName;
                default:
                    throw new InvalidMessageException(msg.ToString());
            }
        }
    }
}