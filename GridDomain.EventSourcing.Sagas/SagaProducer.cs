using System;
using System.Collections.Generic;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaProducer<TSaga> : ISagaProducer<TSaga>
    {
        private readonly Dictionary<Type, Func<object, TSaga>> _factories = new Dictionary<Type, Func<object, TSaga>>();

        public SagaProducer(ISagaDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public TSaga Create(object data)
        {
            var type = data.GetType();
            Func<object, TSaga> factory;
            if (!_factories.TryGetValue(type, out factory))
                throw new CannotFindFactoryForSagaCreation(typeof(TSaga), data);

            return factory.Invoke(data);
        }

        public ISagaDescriptor Descriptor { get; }

        public IReadOnlyCollection<Type> KnownDataTypes => _factories.Keys;

        public void RegisterAll<TFactory, TData>(TFactory factory)
            where TFactory : ISagaFactory<TSaga, SagaStateAggregate<TData>> where TData : ISagaState
        {
            dynamic dynamicfactory = factory;

            foreach (var startMessageType in Descriptor.StartMessages)
            {
                var expectedFactoryType = typeof(ISagaFactory<,>).MakeGenericType(typeof(TSaga), startMessageType);
                if (!expectedFactoryType.IsInstanceOfType(factory))
                    throw new FactoryNotSupportStartMessageException(factory.GetType(), startMessageType);

                //TODO: think how to avoid dynamic call and call method need by message
                Register(startMessageType, msg => (TSaga) dynamicfactory.Create((dynamic) msg));
            }

            Register(factory);
        }

        public void Register<TMessage>(ISagaFactory<TSaga, TMessage> factory)
        {
            Register(typeof(TMessage), m => factory.Create((TMessage) m));
        }

        private void Register(Type dataType, Func<object, TSaga> factory)
        {
            if (_factories.ContainsKey(dataType))
                throw new FactoryAlreadyRegisteredException(dataType);

            _factories[dataType] = factory.Invoke;
        }
    }
}