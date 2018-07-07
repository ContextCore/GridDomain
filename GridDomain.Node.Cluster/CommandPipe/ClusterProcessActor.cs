using Akka.Actor;
using Akka.Cluster.Sharding;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Cluster.CommandPipe
{
    public class ClusterProcessActor<T> : ProcessActor<T> where T : class, IProcessState
    {
        private readonly IActorRef _shardRegion;

        public ClusterProcessActor(IProcess<T> process,
                                   IProcessStateFactory<T> processStateFactory,
                                   string stateActorPath,
                                   IRecycleConfiguration recycle) : base(process,
                                                                         processStateFactory,
                                                                         stateActorPath)
        {
            _shardRegion = ClusterSharding.Get(Context.System)
                                          .ShardRegion(Known.Names.Region(process.GetType()));

            Context.SetReceiveTimeout(recycle.ChildMaxInactiveTime);
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

        protected override object GetShutdownMessage(Shutdown.Request r)
        {
            return r;
        }

        protected override IMessageMetadataEnvelop Envelop(ProcessRedirect processRedirect, IMessageMetadata metadata)
        {
            return new ShardedProcessMessageMetadataEnvelop(processRedirect, processRedirect.ProcessId, typeof(T).BeautyName(), metadata);
        }

        protected override void AwaitingMessageBehavior()
        {
            Receive<ReceiveTimeout>(_ =>
                                    {
                                       // Log.Debug("Going to passivate");
                                        Context.Parent.Tell(new Passivate(Shutdown.Request.Instance));
                                    });
            base.AwaitingMessageBehavior();
        }
    }
}