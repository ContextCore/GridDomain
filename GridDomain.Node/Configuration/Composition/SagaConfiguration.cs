using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using Microsoft.Practices.Unity;

namespace GridDomain.Node.Configuration.Composition
{
    public class SagaConfiguration<TSaga>: IContainerConfiguration where TSaga : ISagaInstance
    {
        protected readonly SagaProducer<TSaga> Producer;

        public SagaConfiguration(Func<object, TSaga> factory, ISagaDescriptor descriptor)
        {
            Producer = new SagaProducer<TSaga>(descriptor);
            foreach (var dataType in descriptor.StartMessages)
                Producer.Register(dataType, factory);
        }

        public SagaConfiguration(SagaProducer<TSaga> producer)
        {
            Producer = producer;
        }

        public void Register<T>(ISagaFactory<TSaga, T> factory)
        {
            Producer.Register(factory);
        }
        public void Register(IUnityContainer container)
        {
            container.RegisterInstance<ISagaProducer<TSaga>>(Producer);
        }

        public static SagaConfiguration<TSaga> New<TFactory, TState>(TFactory factory, ISagaDescriptor descriptor)
                                                                     where TFactory : ISagaFactory<TSaga, TState>
        {
            var conf = new SagaConfiguration<TSaga>(new SagaProducer<TSaga>(descriptor));
            //conf.Register<Guid>(factory); - replaced by aggregate factory inside saga actor
            conf.Register<TState>(factory);
            return conf;
        }
    }
}