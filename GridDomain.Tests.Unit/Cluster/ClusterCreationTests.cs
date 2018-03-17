using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.TestKit.Xunit2;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster
{
    /// <summary>
    /// GIVEN actor system builder
    /// WHEN building a cluster
    /// </summary>
    public class ClusterConfigurationTests : IDisposable
    {
        private ITestOutputHelper _testOutputHelper;
        private AkkaCluster _akkaCluster;

        public class SimpleClusterListener : UntypedActor
        {
            public static IReadOnlyCollection<Member> KnownMemberList => _knownMembers;

            protected ILoggingAdapter Log = Context.GetLogger();
            protected Akka.Cluster.Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);
            private static readonly List<Member> _knownMembers = new List<Member>();

            /// <summary>
            /// Need to subscribe to cluster changes
            /// </summary>
            protected override void PreStart()
            {
                // subscribe to IMemberEvent and UnreachableMember events
                Cluster.Subscribe(Self,
                                  ClusterEvent.InitialStateAsEvents,
                                  new[] {typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember)});
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
                        var mem = up;
                        Log.Info("Member is Up: {0}", mem.Member);
                        _knownMembers.Add(mem.Member);
                        break;
                    case ClusterEvent.UnreachableMember _:
                        var unreachable = (ClusterEvent.UnreachableMember) message;
                        Log.Info("Member detected as unreachable: {0}", unreachable.Member);
                        break;
                    case ClusterEvent.MemberRemoved _:
                        var removed = (ClusterEvent.MemberRemoved) message;
                        Log.Info("Member is Removed: {0}", removed.Member);
                        break;
                    case ClusterEvent.IMemberEvent _:
                        //IGNORE
                        break;
                    default:
                        Unhandled(message);
                        break;
                }
            }
        }

        public ClusterConfigurationTests(ITestOutputHelper output)
        {
            _testOutputHelper = output;
        }

        [Fact]
        public async Task Cluster_can_start()
        {
            var logger = new XUnitAutoTestLoggerConfiguration(_testOutputHelper).CreateLogger();

            _akkaCluster = ActorSystemBuilder.New()
                                             .Cluster("test")
                                             .Seeds(10011)
                                             .Workers(1)
                                             .Build()
                                             .CreateCluster(s => s.AttachSerilogLogging(logger));

            var diagnoseActor = _akkaCluster.RandomNode()
                                            .ActorOf(Props.Create(() => new SimpleClusterListener()));

            //will give cluster time to form
            await Task.Delay(5000);

            var initialSystemAddresses = _akkaCluster.All.Select(s => ((ExtendedActorSystem) s).Provider.DefaultAddress)
                                                     .ToArray();

            var knownClusterMembers = SimpleClusterListener.KnownMemberList;
            var knownClusterAddresses = knownClusterMembers.Select(m => m.Address)
                                                           .ToArray();

            //All members of cluster should be reachable
            Assert.Equal(initialSystemAddresses, knownClusterAddresses);
        }

        public void Dispose()
        {
            _akkaCluster?.Dispose();
        }
    }
}