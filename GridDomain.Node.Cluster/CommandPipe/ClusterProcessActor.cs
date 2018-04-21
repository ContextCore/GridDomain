using Akka.Actor;
using Akka.Cluster.Sharding;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Cluster.CommandPipe {
    public class ClusterProcessActor<T> : ProcessActor<T> where T : class, IProcessState
    {
        private IActorRef _shardRegion;

        public ClusterProcessActor(IProcess<T> process, IProcessStateFactory<T> processStateFactory, string stateActorPath) : base(process, processStateFactory, stateActorPath)
        {
            _shardRegion = ClusterSharding.Get(Context.System)
                                          .ShardRegion(Known.Names.Region(process.GetType()));
        }

        protected override object CreateGetStateMessage()
        {
            return new ClusterGetProcessState(typeof(ProcessStateAggregate<T>), Id);
        }

        protected override IMessageMetadataEnvelop EnvelopStateCommand(ICommand cmd, IMessageMetadata metadata)
        {
            return new ShardedCommandMetadataEnvelop(cmd, metadata);
        }

        protected override IActorRef RedirectActor()
        {
            return _shardRegion;
        }

        protected override IMessageMetadataEnvelop EnvelopRedirect(ProcessRedirect processRedirect, IMessageMetadata metadata)
        {
            return new ShardedProcessMessageMetadataEnvelop(processRedirect, processRedirect.ProcessId, typeof(T).BeautyName(), metadata);
        }
    }
}