using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.Node.Cluster {
    
    
    
    public class ClusterCommandPipe : IMessagesRouter
    {
        private Akka.Cluster.Cluster _cluster;
        private readonly ExtendedActorSystem _actorSystem;

        public ClusterCommandPipe(Akka.Cluster.Cluster cluster)
        {
            _cluster = cluster;
            _actorSystem = _cluster.System;
        }
        
        public async Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor)
        {
            var actorType  = typeof(AggregateActorCell<>).MakeGenericType(descriptor.AggregateType);

            var region = await ClusterSharding.Get(_actorSystem)
                                              .StartAsync(descriptor.AggregateType.BeautyName(),
                                                          Props.Create(actorType),
                                                          ClusterShardingSettings.Create(_actorSystem),
                                                          new ShardedMessageMetadataExtractor());
            
        }

        public Task RegisterProcess(IProcessDescriptor processDescriptor, string name = null)
        {
            throw new NotImplementedException();
        }

        public Task RegisterSyncHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            throw new NotImplementedException();
        }

        public Task RegisterFireAndForgetHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            throw new NotImplementedException();
        }
    }
}