using System;
using System.Collections.Generic;
using Akka.Util.Internal;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node.Configuration.Composition
{
    public class DomainBuilder : IDomainBuilder
    {
        private readonly List<IMessageRouteMap> _maps = new List<IMessageRouteMap>();

        private readonly List<IContainerConfiguration> _containerConfigurations = new List<IContainerConfiguration>();
        public IReadOnlyCollection<IContainerConfiguration> ContainerConfigurations => _containerConfigurations;

        public void Configure(ContainerBuilder container)
        {
            ContainerConfigurations.ForEach(container.Register);

        }

        public void Configure(IMessagesRouter router)
        {
            foreach (var m in _maps)
                m.Register(router)
                 .Wait();
        }

        public void RegisterProcessManager<TState>(IProcessDependencyFactory<TState> processDependenciesfactory) where TState : class, IProcessState
        {
            _containerConfigurations.Add(new ProcessManagerConfiguration<TState>(processDependenciesfactory));
            _maps.Add(processDependenciesfactory.CreateRouteMap());


            //   RegisterStateAggregate<ProcessStateActor<TState>>(container);
            //    container.Register<ProcessStateActor<TState>>(c => new ProcessStateActor<TState>(persistentChildsRecycleConfiguration, process.GetType().BeautyName()));
            //var persistentChildsRecycleConfiguration = _processDependencyFactory.StateDependencyFactory.CreateRecycleConfiguration();
            //container.Register<ProcessManagerHubActor<TState>>(c => new ProcessManagerHubActor<TState>(persistentChildsRecycleConfiguration, process.GetType().BeautyName()));
            //for direct access to process state from repositories and for generalization
            //RegisterAggregate<ProcessStateAggregate<TState>>(processDependenciesfactory.StateDependencyFactory);

            var stateConfig = new ProcessStateAggregateConfiguration<TState>(processDependenciesfactory.StateDependencyFactory);
            _containerConfigurations.Add(stateConfig);
            _containerConfigurations.Add(new AggregateConfiguration<AggregateActor<ProcessStateAggregate<TState>>, ProcessStateAggregate<TState>>(processDependenciesfactory.StateDependencyFactory));

            _maps.Add(processDependenciesfactory.StateDependencyFactory.CreateRouteMap());
        }

      
        public void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate
        {
            _containerConfigurations.Add(new AggregateConfiguration<AggregateActor<TAggregate>, TAggregate>(factory));
            _maps.Add(factory.CreateRouteMap());

        }
        
        public void RegisterHandler<TMessage, THandler>(IMessageHandlerFactory<TMessage, THandler> factory) where THandler : IHandler<TMessage> where TMessage : class, IHaveProcessId, IHaveId
        {
            var cfg = new ContainerConfiguration(c => c.Register<THandler>(ctx => factory.Create(ctx.Resolve<IMessageProcessContext>())),
                                                 c => c.RegisterType<MessageHandleActor<TMessage, THandler>>());
            _containerConfigurations.Add(cfg);
            _maps.Add(factory.CreateRouteMap());
        }
    }
}