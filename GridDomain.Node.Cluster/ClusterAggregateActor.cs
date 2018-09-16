using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using GridDomain.Configuration;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Cluster.CommandPipe;

namespace GridDomain.Node.Cluster
{

    public class ClusterAggregateActorCell<T>:DICellActor<ClusterAggregateActor<T>> where T : class, IAggregate
    {
    
    }


    public class ClusterAggregateActor<T> : AggregateActor<T> where T : class, IAggregate
    {
        public ClusterAggregateActor(
                                     ISnapshotsPersistencePolicy snapshotsPersistencePolicy,
                                     IAggregateFactory aggregateConstructor,
                                     ISnapshotFactory snapshotFactoryConstructor,
                                     IActorRef customHandlersActor,
                                     IRecycleConfiguration recycle
                                     ) : base(
                                                                           snapshotsPersistencePolicy,
                                                                           aggregateConstructor,
                                                                           snapshotFactoryConstructor,
                                                                           customHandlersActor)
        {
            Context.SetReceiveTimeout(recycle.ChildMaxInactiveTime);
            
        }

        protected override void AwaitingCommandBehavior()
        {
            Command<ReceiveTimeout>(_ =>
                                    {
                                        
                                        Log.Debug("Going to passivate");
                                        Context.Parent.Tell(new Passivate(Shutdown.Request.Instance));
                                    });
            base.AwaitingCommandBehavior();
        }
    }
}