using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster.Sharding;
using Autofac;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;
using GridDomain.Common;
using GridDomain.Domains;
using GridDomain.EventHandlers;
using GridDomain.EventHandlers.Akka;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.Cluster.CommandGrouping;
using GridDomain.Node.Akka.Extensions.Aggregates;

namespace GridDomain.Node.Akka.Cluster
{

    public class ClusterDomainBuilder : IDomainBuilder
    {
        private readonly ActorSystem _system;
        private readonly ContainerBuilder _containerBuilder;

 
        public ClusterDomainBuilder(ActorSystem system, ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
            _system = system;
            _system.InitAggregatesExtension();
            _system.InitEventHandlersExtension(_containerBuilder);
        }

        public Task RegisterAggregate<TAggregate>(IAggregateConfiguration<TAggregate> configuration)
            where TAggregate : class, IAggregate
        {
            return _system.GetAggregatesExtension().RegisterAggregate(configuration);
        }

        public Task RegisterEventHandler<TEvent, THandler>() where THandler : IEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public Task<IDomain> Build()
        {
            _system.GetEventHandlersExtension().FinishRegistration();
            _system.GetAggregatesExtension().FinishRegistration();
            return _system.GetAggregatesExtension().Start();
        }

        public void RegisterCommandHandler<T>(Func<ICommandHandler<ICommand>, T> proxyBuilder)
        {
             _system.GetAggregatesExtension().RegisterCommandHandler(proxyBuilder);
        }

        public void RegisterCommandsResultAdapter<TAggregate>(ICommandsResultAdapter adapter) where TAggregate : IAggregate
        {
            _system.GetAggregatesExtension().RegisterCommandsResultAdapter<TAggregate>(adapter);
        }

    }
}