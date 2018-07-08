using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Tests.Unit.Cluster.CommandsExecution;

namespace GridDomain.Tests.Unit.Cluster.ClusterConf {
    public class SimpleClusterListener : UntypedActor
    {
        protected ILoggingAdapter Log = Context.GetSeriLogger();
        protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);
        private  readonly List<Member> _knownMembers = new List<Member>();
        private int _maxMembers;
        private IActorRef _actorRef;

        /// <summary>
        /// Need to subscribe to cluster changes
        /// </summary>
        protected override void PreStart()
        {
            // subscribe to IMemberEvent and UnreachableMember events
            Cluster.Subscribe(Self,
                              ClusterEvent.InitialStateAsEvents,
                              new[]
                              {
                                  typeof(ClusterEvent.IMemberEvent),
                                  typeof(ClusterEvent.UnreachableMember),
                              });
        }

        /// <summary>
        /// Re-subscribe on restart
        /// </summary>
        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        protected override void OnReceive(object message)
        {
            
            switch (message)
            {
                
                case GetResult r:
                    _maxMembers = r.MaxMembers;
                    _actorRef = Sender;
                    break;
                case ClusterEvent.MemberUp up:
                    Log.Info("Member is Up: {0}", up.Member);
                    AddMember(up.Member);
                    break;
                case ClusterEvent.UnreachableMember unreachable:
                    Log.Info("Member detected as unreachable: {0}", unreachable.Member);
                    break;
                case ClusterEvent.MemberRemoved removed:
                    Log.Info("Member is Removed: {0}", removed.Member);
                    break;
                case ClusterEvent.IMemberEvent a:
                    
                    break;
                case ClusterEvent.CurrentClusterState state:
                    foreach(var m in state.Members)
                        AddMember(m);
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }

        private void AddMember(Member m)
        {
            _knownMembers.Add(m);
            if (_knownMembers.Count >= _maxMembers) 
                _actorRef.Tell(new MembersExplored(_knownMembers));
        }
        
    }
}