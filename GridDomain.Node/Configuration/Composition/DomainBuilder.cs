using System;
using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.ProcessManagers;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class DomainBuilder : IDomainBuilder
    {
        private readonly List<IMessageRouteMap> _maps = new List<IMessageRouteMap>();
        public IReadOnlyCollection<IMessageRouteMap> MessageRouteMaps => _maps;

        private readonly List<IContainerConfiguration> _containerConfigurations = new List<IContainerConfiguration>();
        public IReadOnlyCollection<IContainerConfiguration> ContainerConfigurations => _containerConfigurations;
        
        public void RegisterProcessManager<TState>(IProcessManagerDependencyFactory<TState> factory) where TState : class, IProcessState
        {
            _containerConfigurations.Add(new ProcessManagerConfiguration<TState>(c => factory.CreateCatalog(),
                                                                         factory.ProcessName,
                                                                         () => factory.StateDependencyFactory.CreatePersistencePolicy(),
                                                                         factory.StateDependencyFactory.CreateFactory(),
                                                                         factory.StateDependencyFactory.CreateRecycleConfiguration()));
            _maps.Add(factory.CreateRouteMap());
        }

        public void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate
        {
            _containerConfigurations.Add(new AggregateConfiguration<AggregateActor<TAggregate>, TAggregate>(c => factory.CreateCommandsHandler(),
                                                                                                            factory.CreatePersistencePolicy,
                                                                                                            factory.CreateFactory(),
                                                                                                            factory.CreateRecycleConfiguration()));
            _maps.Add(factory.CreateRouteMap());

        }

        public void RegisterHandler<TMessage, THandler>(IMessageHandlerFactory<TMessage, THandler> factory) where THandler : IHandler<TMessage>
        {
            var cfg = new ContainerConfiguration(c => c.RegisterType<THandler>(new InjectionFactory(cont => factory.Create(c.Resolve<IMessageProcessContext>()))));
            _containerConfigurations.Add(cfg);
            _maps.Add(factory.CreateRouteMap());
        }
    }
}