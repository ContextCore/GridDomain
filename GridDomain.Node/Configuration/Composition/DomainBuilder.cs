using System;
using System.Collections.Generic;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class DomainBuilder : IDomainBuilder
    {
        private readonly List<IContainerConfiguration> _containerConfigurations = new List<IContainerConfiguration>();
        public IReadOnlyCollection<IContainerConfiguration> ContainerConfigurations => _containerConfigurations;

        public void RegisterSaga<TProcess, TSaga>(ISagaDependencyFactory<TSaga, TProcess> factory) where TProcess : class, ISagaState
                                                                                                   where TSaga : Process<TProcess>
        {
            _containerConfigurations.Add(SagaConfiguration.New(factory));
        }

        public void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate
        {
            _containerConfigurations.Add(AggregateConfiguration.New(factory));
        }

        public void RegisterHandler<TMessage, THandler>(IMessageHandlerFactory<TMessage, THandler> factory) where THandler : IHandler<TMessage>
        {
            var cfg = new ContainerConfiguration(c => c.RegisterType<THandler>(
                                                                               new InjectionFactory(cont => factory.Create(c.Resolve<IMessageProcessContext>()))));
            _containerConfigurations.Add(cfg);
        }

        public void RegisterHandler<TMessage, THandler>(IMessageHandlerWithMetadataFactory<TMessage, THandler> factory) where THandler : IHandlerWithMetadata<TMessage>
        {
            var cfg = new ContainerConfiguration(c => c.RegisterType<THandler>(
                                                                               new InjectionFactory(cont => factory.Create(c.Resolve<IMessageProcessContext>()))));
            _containerConfigurations.Add(cfg);
        }
    }
}