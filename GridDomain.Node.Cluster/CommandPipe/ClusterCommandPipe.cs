using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net.Security;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.Cluster.Sharding;
using Akka.DI.Core;
using Akka.Event;
using Akka.Routing;
using Akka.Util.Internal;
using Autofac;
using Autofac.Core;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Cluster.CommandPipe.CommandGrouping;
using GridDomain.ProcessManagers.DomainBind;
using GridDomain.ProcessManagers.State;
using GridDomain.Transport.Extension;
using Serilog;

namespace GridDomain.Node.Cluster.CommandPipe
{
    public class ClusterCommandPipe : IActorCommandPipe
    {
        public ActorSystem System { get; }
        public IActorRef ProcessesPipeActor { get; private set; }
        public IActorRef HandlersPipeActor { get; private set; }
        public IActorRef CommandExecutor { get; protected set; }

        readonly Dictionary<string, IActorRef> _aggregatesRegions = new Dictionary<string, IActorRef>();
        readonly List<string> _processAggregatesRegionPaths = new List<string>();

        readonly MessageMap _messageMap = new MessageMap();

        private readonly ILogger _log;

        private readonly List<Func<Task>> _delayedRegistrations = new List<Func<Task>>();
        private readonly List<IProcessDescriptor> _processDescriptors = new List<IProcessDescriptor>();

        public ClusterCommandPipe(ActorSystem system, ILogger log)
        {
            _log = log;
            System = system;
        }

        public Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor)
        {
            Type aggregateType = descriptor.AggregateType;
            _delayedRegistrations.Add(() => RegisterAggregateByType(aggregateType, typeof(ClusterAggregateActor<>).MakeGenericType(aggregateType)));
            return Task.CompletedTask;
        }

        private async Task RegisterAggregateByType(Type aggregateType, Type actorType)
        {

            var supervisionPolicy = new OneForOneStrategy(ex =>
                                                          {
                                                              switch (ex)
                                                              {
                                                                  case CommandExecutionFailedException cf:
                                                                      return Directive.Restart;
                                                                  case CommandAlreadyExecutedException cae:
                                                                      return Directive.Restart;
                                                                  default:
                                                                      return Directive.Stop;
                                                              }
                                                          });
            
            var region = await ClusterSharding.Get(System)
                                              .StartAsync(Known.Names.Region(aggregateType),
                                                          System.DI()
                                                                .Props(actorType)
                                                                .WithSupervisorStrategy(supervisionPolicy),
                                                          ClusterShardingSettings.Create(System),
                                                          new ShardedMessageMetadataExtractor());

            if (_aggregatesRegions.ContainsKey(Known.Names.Region(aggregateType)))
                throw new InvalidOperationException("Cannot add dublicate region path: " + region.Path);

            _aggregatesRegions.Add(Known.Names.Region(aggregateType), region);
        }

        public Task RegisterProcess(IProcessDescriptor processDescriptor, string name = null)
        {
            _processDescriptors.Add(processDescriptor);
            _delayedRegistrations.Add(async () =>
                                      {
                                          Type actorType = typeof(ClusterProcessStateActor<>).MakeGenericType(processDescriptor.StateType);
                                          Type aggregateType = typeof(ProcessStateAggregate<>).MakeGenericType(processDescriptor.StateType);

                                          await RegisterAggregateByType(aggregateType, actorType);

                                          var processActorType = typeof(ClusterProcessActorCell<>).MakeGenericType(processDescriptor.StateType);

                                          var region = await ClusterSharding.Get(System)
                                                                            .StartAsync(Known.Names.Region(processDescriptor.ProcessType),
                                                                                        System.DI()
                                                                                              .Props(processActorType),
                                                                                        ClusterShardingSettings.Create(System),
                                                                                        new ShardedMessageMetadataExtractor());
                                          var path = region.Path.ToString();

                                          if (_processAggregatesRegionPaths.Contains(path))
                                              throw new InvalidOperationException("Cannot add dublicate region path: " + path);
                                          _processAggregatesRegionPaths.Add(path);
                                      });

           
            return Task.CompletedTask;
        }

        public Task RegisterSyncHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            return _messageMap.RegisterSync<TMessage, THandler>();
        }

        public Task RegisterFireAndForgetHandler<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            return _messageMap.RegisterFireAndForget<TMessage, THandler>();
        }

        public async Task StartRoutes()
        {
            foreach (var delayed in _delayedRegistrations)
                await delayed();

            BuildProcessManagerRoutes();

            BuildMessageHandlers();

            BuildAggregateCommandingRoutes();
        }

        private void BuildProcessManagerRoutes()
        {
            var routingGroup = new ConsistentHashingPool(10)
                .WithHashMapping(m =>
                                 {
                                     if (m is IMessageMetadataEnvelop env)
                                     {
                                         if(env.Message is DomainEvent evt)
                                             return evt.SourceId;
                                         if (env.Message is IFault flt)
                                         {
                                             if (flt.Message is DomainEvent e)
                                                 return e.SourceId;
                                             if (flt.Message is ICommand c)
                                                 return c.AggregateId;
                                         }
                                     }

                                     throw new InvalidMessageException(m.ToString());
                                 });

            var processesProps = System.DI()
                                       .Props(typeof(ClusterProcessPipeActor));

            ProcessesPipeActor = System.ActorOf(processesProps.WithRouter(routingGroup), "Processes");
        }

        private void BuildMessageHandlers()
        {
            var routingPool = new ConsistentHashingPool(10)
                .WithHashMapping(m =>
                                 {
                                     if (m is IMessageMetadataEnvelop env)
                                     {
                                         return env.Metadata.CorrelationId;
                                     }
                                    

                                     throw new InvalidMessageException(m.ToString());
                                 });

            var clusterRouterPool = new ClusterRouterPool(routingPool, new ClusterRouterPoolSettings(10, 1, true));

            var handlerActorProps = Props.Create<ClusterHandlersPipeActorCell>()
                                          .WithRouter(clusterRouterPool);

            HandlersPipeActor = System.ActorOf(handlerActorProps, nameof(ClusterHandlersPipeActorCell));
        }                                                          

        protected virtual void BuildAggregateCommandingRoutes()
        {
            var routingGroup = new ConsistentMapGroup(_aggregatesRegions)
                .WithMapping(m =>
                             {
                                 if (m is IShardedMessageMetadataEnvelop env && env.Message is ICommand cmd)
                                 {
                                     if(_aggregatesRegions.ContainsKey(cmd.AggregateName))
                                          return cmd.AggregateName;
                                     throw new UnknownCommandExeption();
                                 }

                                 throw new InvalidMessageException(m.ToString());
                             });

            CommandExecutor = System.ActorOf(Props.Empty.WithRouter(routingGroup), "Aggregates");
        }

        public void Dispose()
        {
            System.Dispose();
        }

        public void Register(ContainerBuilder container)
        {
            container.RegisterType<ClusterProcessPipeActor>()
                     .WithParameters(new Parameter[]
                                     {
                                         new TypedParameter(typeof(IReadOnlyCollection<IProcessDescriptor>), _processDescriptors),
                                         //{akka://AutoTest/user/Aggregates}
                                         new TypedParameter(typeof(string), "/user/Aggregates")
                                     });

            container.RegisterType<ClusterHandlersPipeActor>()
                     .WithParameters(new Parameter[]
                                     {
                                         new TypedParameter(typeof(MessageMap), _messageMap),
                                         new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IActorRef),
                                                               (pi, ctx) => ctx.ResolveNamed<IActorRef>(Actors.CommandPipe.ProcessesPipeActor.ProcessManagersPipeActorRegistrationName))
                                     });

            container.Register((c, p) => ProcessesPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.ProcessesPipeActor.ProcessManagersPipeActorRegistrationName);

            container.Register((c, p) => HandlersPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.HandlersPipeActor.CustomHandlersProcessActorRegistrationName);
        }
    }
}