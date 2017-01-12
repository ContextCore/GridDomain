using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;
using Quartz.Simpl;

namespace GridDomain.Node.Configuration.Composition
{

    public class SagaConfiguration
    {
        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaStateAggregate<TData>> Instance<TSaga, TData>(ISagaFactory<ISagaInstance<TSaga, TData>, SagaStateAggregate<TData>> factory, ISagaDescriptor descriptor, Func<ISnapshotsPersistencePolicy> snapShotsPolicy = null)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
        {
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor);
            producer.RegisterAll<ISagaFactory<ISagaInstance<TSaga, TData>, SagaStateAggregate<TData>>,TData>(factory);
            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaStateAggregate<TData>>(producer, snapShotsPolicy);
        }
        public static IContainerConfiguration Instance<TSaga, TData,TFactory>(ISagaDescriptor descriptor, Func<ISnapshotsPersistencePolicy> snapShotsPolicy = null, IConstructAggregates factory = null)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
                 where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaStateAggregate<TData>>
        {
            return new SagaConfiguration<TSaga, TData, TFactory>(descriptor,snapShotsPolicy, factory);
        }
    }


    public class SagaConfiguration<TSaga, TState, TFactory> : IContainerConfiguration
        where TSaga : Saga<TState>
        where TState : class, ISagaState
        where TFactory : ISagaFactory<ISagaInstance<TSaga, TState>, SagaStateAggregate<TState>>
    {
        private readonly Func<ISnapshotsPersistencePolicy> _snapShotsPolicy;
        private IConstructAggregates _aggregateFactory;
        private readonly ISagaDescriptor _descriptor;

        public SagaConfiguration(ISagaDescriptor descriptor, Func<ISnapshotsPersistencePolicy> snapShotsPolicy = null, IConstructAggregates factory = null)
        {
            _descriptor = descriptor;
            _aggregateFactory = factory;
            _snapShotsPolicy = snapShotsPolicy;
        }

        public void Register(IUnityContainer container)
        {
            var sagaSpecificRegistrationsName = typeof(TSaga).Name;

            container.RegisterInstance<IConstructAggregates>(sagaSpecificRegistrationsName, _aggregateFactory);
            container.RegisterType<ISnapshotsPersistencePolicy>(sagaSpecificRegistrationsName, new InjectionFactory(c => _snapShotsPolicy()));
            container.RegisterType<ISagaProducer<ISagaInstance<TSaga, TState>>>(sagaSpecificRegistrationsName,
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(
                    c =>
                    {
                        var factory = c.Resolve<TFactory>();
                        var producer = new SagaProducer<ISagaInstance<TSaga, TState>>(_descriptor);
                        producer.RegisterAll<TFactory, TState>(factory);
                        return producer;
                    }));

           container.RegisterType<SagaActor<ISagaInstance<TSaga, TState>, SagaStateAggregate<TState>>>();
        }
    }


    public class SagaConfiguration<TSaga,TState> : IContainerConfiguration where TSaga : class, ISagaInstance where TState : AggregateBase
    {
        private readonly SagaProducer<TSaga> _producer;
        private readonly Func<ISnapshotsPersistencePolicy> _snapshotsPolicyFactory;
        private readonly IConstructAggregates _factory;

        public SagaConfiguration(SagaProducer<TSaga> producer, Func<ISnapshotsPersistencePolicy> snapShotsPolicy = null, IConstructAggregates factory = null)
        {
            _factory = factory ??  new AggregateFactory();
            _snapshotsPolicyFactory = snapShotsPolicy ?? (() => new NoSnapshotsPersistencePolicy());
            _producer = producer;
        }
        
        public void Register(IUnityContainer container)
        {
            container.RegisterInstance<ISagaProducer<TSaga>>(_producer);
            container.RegisterInstance(_producer);

            var snapshotsPolicyRegistrationName = typeof(TSaga).Name;

            container.RegisterType<ISnapshotsPersistencePolicy>(snapshotsPolicyRegistrationName, new InjectionFactory(c => _snapshotsPolicyFactory()));
            container.RegisterType<SagaActor<TSaga, TState>>(
                new InjectionConstructor(new ResolvedParameter<ISagaProducer<TSaga>>(),
                                         new ResolvedParameter<IPublisher>(), 
                                         new ResolvedParameter<ISnapshotsPersistencePolicy>(snapshotsPolicyRegistrationName),
                                         _factory));

            container.RegisterInstance(snapshotsPolicyRegistrationName, _factory);
        }
    }
}