using System;
using System.Reflection;
using Akka.Actor;
using Autofac;
using Autofac.Core;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.Configuration.SnapshotPolicies;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.PersistentHub;

namespace GridDomain.Node.Configuration.Composition
{
    public class AggregateConfiguration<TAggregateActor, TAggregate> : IContainerConfiguration
        where TAggregate : Aggregate
    {
        protected readonly IAggregateDependencyFactory<TAggregate> AggregateDependencyFactory;

        public AggregateConfiguration(IAggregateDependencyFactory<TAggregate> factory)
        {
            AggregateDependencyFactory = factory;
        }
       
        public void Register(ContainerBuilder container)
        {
            RegisterHub(container);

            container.RegisterType<TAggregateActor>()
                     .WithParameters(CreateParametersRegistration());
        }

        protected virtual Parameter[] CreateParametersRegistration()
        {
            return new Parameter[] { 
                                       new TypedParameter(typeof(IAggregateCommandsHandler<TAggregate>), AggregateDependencyFactory.CreateCommandsHandler()),
                                       new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IPublisher),
                                                             (pi, ctx) => ctx.Resolve<IPublisher>()),
                                       new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(ISnapshotsPersistencePolicy),
                                                             (pi, ctx) => ((Func<ISnapshotsPersistencePolicy>) AggregateDependencyFactory.CreatePersistencePolicy)()),
                                       new TypedParameter(typeof(IConstructAggregates), AggregateDependencyFactory.CreateAggregateFactory()),
                                       new TypedParameter(typeof(IConstructSnapshots), AggregateDependencyFactory.CreateSnapshotsFactory()),
                                       new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IActorRef),
                                                             (pi, ctx) => ctx.ResolveNamed<IActorRef>(HandlersPipeActor.CustomHandlersProcessActorRegistrationName))
                                   };
        }

        protected virtual void RegisterHub(ContainerBuilder container)
        {
            container.Register<AggregateHubActor<TAggregate>>(c => new AggregateHubActor<TAggregate>(AggregateDependencyFactory.CreateRecycleConfiguration()));
        }
    }
}