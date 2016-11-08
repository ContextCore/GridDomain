using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;
using Quartz.Simpl;

namespace GridDomain.Node.Configuration.Composition
{

    public class SagaConfiguration
    {
        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>> Instance<TSaga, TData>
            (SagaProducer<ISagaInstance<TSaga, TData>> producer, Func<SnapshotsSavePolicy> snapShotsPolicy = null) where TData : ISagaState
        {
            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer,snapShotsPolicy);
        }



        public static SagaConfiguration<ISagaInstance<TSaga,TData>, SagaDataAggregate<TData>> Instance<TSaga, TData, TFactory, TStartMessage>(ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
         where TSaga : Saga<TData>
         where TData : class, ISagaState
         where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                          ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessage>, new()
        {
            return State<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>, TFactory, TStartMessage>(descriptor,snapShotsPolicy);
        }

        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>> Instance<TSaga, TData, TFactory, TStartMessageA, TStartMessageB, TStartMessageC>(ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
               where TSaga : Saga<TData>
               where TData : class, ISagaState
               where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageA>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageB>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageC>,
                               new()
        {
            var factory = new TFactory();
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor);
            producer.Register<TStartMessageA>(factory);
            producer.Register<TStartMessageB>(factory);
            producer.Register<TStartMessageC>(factory);
            producer.Register<SagaDataAggregate<TData>>(factory);

            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer,snapShotsPolicy);

        }

        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>> Instance<TSaga, TData, TFactory, TStartMessageA, TStartMessageB>(ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
               where TSaga : Saga<TData>
               where TData : class, ISagaState
               where TFactory : ISagaFactory<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageA>,
                                ISagaFactory<ISagaInstance<TSaga, TData>, TStartMessageB>,
                               new()
        {
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor);
            var factory = new TFactory();

            producer.Register<TStartMessageA>(factory);
            producer.Register<TStartMessageB>(factory);
            producer.Register<SagaDataAggregate<TData>>(factory);

            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer,snapShotsPolicy);
        }


        public static SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>> Instance<TSaga, TData>(Func<object, ISagaInstance<TSaga, TData>> factory, ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
                 where TSaga : Saga<TData>
                 where TData : class, ISagaState
        {
            var producer = new SagaProducer<ISagaInstance<TSaga, TData>>(descriptor, factory);
            return new SagaConfiguration<ISagaInstance<TSaga, TData>, SagaDataAggregate<TData>>(producer, snapShotsPolicy);
        }

        public static SagaConfiguration<TSaga, TState> State<TSaga, TState, TFactory, TStartMessage>(ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
                where TFactory : ISagaFactory<TSaga, TState>,
                                 ISagaFactory<TSaga, TStartMessage>,
                                 new()

                where TSaga : class, ISagaInstance
                where TState : AggregateBase
        {
            var producer = new SagaProducer<TSaga>(descriptor);
            var factory = new TFactory();
            producer.Register<TState>(factory);
            producer.Register<TStartMessage>(factory);

            return new SagaConfiguration<TSaga, TState>(producer,snapShotsPolicy);
        }

    }

    public class SagaConfiguration<TSaga,TState> : IContainerConfiguration where TSaga : class, ISagaInstance where TState : AggregateBase
    {
        protected readonly SagaProducer<TSaga> Producer;
        private readonly Func<SnapshotsSavePolicy> _snapshotsPolicyFactory;

        public SagaConfiguration(SagaProducer<TSaga> producer, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
        {
            _snapshotsPolicyFactory = snapShotsPolicy ?? (() => new DefaultSnapshotsSavePolicy());
            Producer = producer;
        }
        
        public void Register(IUnityContainer container)
        {
            container.RegisterInstance<ISagaProducer<TSaga>>(Producer);
            container.RegisterInstance(Producer);
            container.RegisterType<SnapshotsSavePolicy>(typeof(TSaga).Name,new InjectionFactory(c => _snapshotsPolicyFactory()));
            container.RegisterType<SagaActor<TSaga, TState>>(
                new InjectionConstructor(new ResolvedParameter<ISagaProducer<TSaga>>(),
                                         new ResolvedParameter<IPublisher>(), 
                                         new ResolvedParameter<SnapshotsSavePolicy>(typeof(TSaga).Name)));
        }



    }
}