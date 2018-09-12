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
        protected readonly IAggregateDependencies<TAggregate> AggregateDependencies;

        public AggregateConfiguration(IAggregateDependencies<TAggregate> factory)
        {
            AggregateDependencies = factory;
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
                                       new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IPublisher),
                                                             (pi, ctx) => ctx.Resolve<IPublisher>()),
                                       new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(ISnapshotsPersistencePolicy),
                                                             (pi, ctx) =>  AggregateDependencies.SnapshotPolicy),
                                       new TypedParameter(typeof(IAggregateFactory), AggregateDependencies.AggregateFactory),
                                       new TypedParameter(typeof(ISnapshotFactory), AggregateDependencies.SnapshotFactory),
                                       new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(IActorRef),
                                                             (pi, ctx) => ctx.ResolveNamed<IActorRef>(HandlersPipeActor.CustomHandlersProcessActorRegistrationName))
                                   };
        }

        protected virtual void RegisterHub(ContainerBuilder container)
        {
            container.Register<AggregateHubActor<TAggregate>>(c => new AggregateHubActor<TAggregate>(AggregateDependencies.RecycleConfiguration));
        }
    }
}