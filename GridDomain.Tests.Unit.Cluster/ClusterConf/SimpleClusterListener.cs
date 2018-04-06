using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using GridDomain.Node;

namespace GridDomain.Tests.Unit.Cluster.ClusterConf {
    public class SimpleClusterListener : UntypedActor
    {
        public static IReadOnlyCollection<Member> KnownMemberList => _knownMembers;
        public static IReadOnlyCollection<Member> KnownSeedsList => _knownSeedsMembers;

        protected ILoggingAdapter Log = Context.GetSeriLogger();
        protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);
        private static readonly List<Member> _knownMembers = new List<Member>();
        private static readonly List<Member> _knownSeedsMembers = new List<Member>();

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
                case ClusterEvent.MemberUp up:
                    Log.Info("Member is Up: {0}", up.Member);
                    _knownMembers.Add(up.Member);
                    break;
                case ClusterEvent.UnreachableMember unreachable:
                    Log.Info("Member detected as unreachable: {0}", unreachable.Member);
                    break;
                case ClusterEvent.MemberRemoved removed:
                    Log.Info("Member is Removed: {0}", removed.Member);
                    break;
                case ClusterEvent.IMemberEvent a:
                    var evt = a;
                    break;
                case ClusterEvent.CurrentClusterState state:
                    _knownMembers.AddRange(state.Members);
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }
    }
}