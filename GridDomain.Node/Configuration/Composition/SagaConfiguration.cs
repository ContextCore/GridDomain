using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;
using Quartz.Simpl;

namespace GridDomain.Node.Configuration.Composition
{
    public class SagaConfiguration<TSaga,TState> : IContainerConfiguration where TSaga : class, ISagaInstance where TState : AggregateBase
    {
        protected readonly SagaProducer<TSaga> Producer;
        private readonly Func<SnapshotsSavePolicy> _snapshotsPolicyFactory;

       //public SagaConfiguration(Func<object, TSaga> factory, ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapShotsPolicy = null) :this(new SagaProducer<TSaga>(descriptor), snapShotsPolicy )
       //{
       //    foreach (var dataType in descriptor.StartMessages)
       //        Producer.Register(dataType, factory);
       //}

        public SagaConfiguration(SagaProducer<TSaga> producer, Func<SnapshotsSavePolicy> snapShotsPolicy = null)
        {
            _snapshotsPolicyFactory = snapShotsPolicy ?? (() => new DefaultSnapshotsSavePolicy());
            Producer = producer;
        }

        public void Register<T>(ISagaFactory<TSaga, T> factory)
        {
            Producer.Register(factory);
        }
        public void Register(IUnityContainer container)
        {
            container.RegisterInstance<ISagaProducer<TSaga>>(Producer);
            container.RegisterInstance(Producer);
            container.RegisterType<SnapshotsSavePolicy>(typeof(TSaga).Name,new InjectionFactory(c => _snapshotsPolicyFactory()));
            container.RegisterType<SagaActor<TSaga, TState>>(
                new InjectionConstructor(new ResolvedParameter<ISagaProducer<TSaga>>(),
                                         new ResolvedParameter<IPublisher>(),  new ResolvedParameter<SnapshotsSavePolicy>(typeof(TSaga).Name)));
        }

       //public static SagaConfiguration<TSaga,TState> New<TFactory>(TFactory factory, ISagaDescriptor descriptor, Func<SnapshotsSavePolicy> snapshotsPolicy = null)
       //                                                             where TFactory : ISagaFactory<TSaga, TState> 
       //{
       //    var conf = new SagaConfiguration<TSaga, TState>(new SagaProducer<TSaga>(descriptor), snapshotsPolicy);
       //    conf.Register(factory);
       //    return conf;
       //}
    }
}