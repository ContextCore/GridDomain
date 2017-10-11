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
            foreach (var m in (IReadOnlyCollection<IMessageRouteMap>) _maps)
                m.Register(router)
                 .Wait();
        }

        public void RegisterProcessManager<TState>(IProcessDependencyFactory<TState> factory) where TState : class, IProcessState
        {
            _containerConfigurations.Add(new ProcessManagerConfiguration<TState>(factory.CreateStateFactory,
                                                                                 factory.CreateProcess,
                                                                                 factory.ProcessName,
                                                                                 () => factory.StateDependencyFactory.CreatePersistencePolicy(),
                                                                                 factory.StateDependencyFactory.CreateAggregateFactory(),
                                                                                 factory.StateDependencyFactory.CreateRecycleConfiguration()));
            _maps.Add(factory.CreateRouteMap());
        }

        public void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate
        {
            _containerConfigurations.Add(new AggregateConfiguration<AggregateActor<TAggregate>, TAggregate>(factory.CreateCommandsHandler(),
                                                                                                            factory.CreatePersistencePolicy,
                                                                                                            factory.CreateAggregateFactory(),
                                                                                                            factory.CreateRecycleConfiguration()));
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