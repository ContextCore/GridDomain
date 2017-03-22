using System;
using CommonDomain;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class SagaConfiguration<TSaga, TState, TFactory> : IContainerConfiguration where TSaga : Process<TState>
                                                                                      where TState : class, ISagaState
                                                                                      where TFactory : IFactory<ISaga<TState>, TState>
    {
        private readonly IConstructAggregates _aggregateFactory;
        private readonly ISagaDescriptor _descriptor;
        private readonly Func<ISnapshotsPersistencePolicy> _snapShotsPolicy;

        public SagaConfiguration(ISagaDescriptor descriptor,
                                 Func<ISnapshotsPersistencePolicy> snapShotsPolicy = null,
                                 IConstructAggregates factory = null)
        {
            _descriptor = descriptor;
            _aggregateFactory = factory ?? new AggregateFactory();
            _snapShotsPolicy = snapShotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
        }

        public void Register(IUnityContainer container)
        {
            var sagaSpecificRegistrationsName = typeof(TSaga).Name;
            container.RegisterInstance(sagaSpecificRegistrationsName, _aggregateFactory);
            container.RegisterType<ISnapshotsPersistencePolicy>(sagaSpecificRegistrationsName,
                                                                new InjectionFactory(c => _snapShotsPolicy()));

            container.RegisterType<ISagaProducer<TState>>(new ContainerControlledLifetimeManager(),
                                                          new InjectionFactory(c =>
                                                                               {
                                                                                   var factory = c.Resolve<TFactory>();
                                                                                   var producer = new SagaProducer<TState>(_descriptor);
                                                                                   producer.RegisterAll(factory);
                                                                                   return producer;
                                                                               }));

            container.RegisterType<SagaActor<TState>>();

            var sagaStateConfig = new SagaStateConfiguration<TState>();
            sagaStateConfig.Register(container);
        }
    }

    public class SagaStateConfiguration<TState> : AggregateBaseConfiguration<SagaStateActor<TState>, SagaStateAggregate<TState>, SagaStateCommandHandler<TState>>
        where TState : ISagaState
    {
        public SagaStateConfiguration(Func<ISnapshotsPersistencePolicy> snapshotsPolicy = null,
                                      Func<IMemento, SagaStateAggregate<TState>> snapshotsFactory = null) : base(snapshotsPolicy, snapshotsFactory) {}
    }
}