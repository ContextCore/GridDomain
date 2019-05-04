using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using Akka.Util;

namespace GridDomain.Node.Akka.Cluster.CommandGrouping {
    public class ConsistentMapGroup: Group
    {
        private readonly IDictionary<string, string> _keyedPaths;
        private Dictionary<string, Routee> _routees;
        private ConsistentMapping _mapping;
        private readonly string[] _paths;

        public ConsistentMapGroup(IDictionary<string,string> keyedPaths):base(new string[]{},null)
        {
            _keyedPaths = keyedPaths;
            _paths = _keyedPaths.Values.ToArray();
        }
        public ConsistentMapGroup(IDictionary<string,IActorRef> keyedActors):this(keyedActors.ToDictionary(p => p.Key, p=>p.Value.Path.ToString()))
        {
            _routees = keyedActors.ToDictionary(p => p.Key, p => Routee.FromActorRef(p.Value));
        }
        
        public ConsistentMapGroup WithMapping(ConsistentMapping mapper)
        {
            _mapping = mapper;
            return this;
        }

        public override Router CreateRouter(ActorSystem system)
        {
            if(_routees == null)
                _routees = _keyedPaths.ToDictionary(p => p.Key, p => (Routee) new ActorSelectionRoutee(system.ActorSelection(p.Value)));
           
            return new Router(new ConsistentMapLogic(system, _routees, _mapping));
        }
      

        public override ISurrogate ToSurrogate(ActorSystem system)
        {
            return new ConsistentGroupSurrogate(){Paths = _keyedPaths};
        }
           
        public class ConsistentGroupSurrogate : ISurrogate
        {
            public ISurrogated FromSurrogate(ActorSystem system)
            {
                return new ConsistentMapGroup(Paths);
            }

            /// <summary>
            /// The actor paths used by this router during routee selection.
            /// </summary>
            public IDictionary<string,string> Paths { get; set; }
        }

        public override IEnumerable<string> GetPaths(ActorSystem system)
        {
            return _paths;
        }
    }
}