using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaProducer<TSaga> : ISagaProducer<TSaga> where TSaga : ISagaInstance
    {
        private readonly Dictionary<Type, Func<object, TSaga>> _factories = new Dictionary<Type,Func<object,TSaga>>();

        public SagaProducer(ISagaDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public void Register<TMessage>(ISagaFactory<TSaga, TMessage> factory)
        {
            Register(typeof(TMessage), m => factory.Create((TMessage) m));
        }

        public void Register(Type dataType, Func<object, TSaga> factory)
        {
            if (_factories.ContainsKey(dataType))
                throw new FactoryAlreadyRegisteredException(dataType);

            _factories[dataType] = factory.Invoke;
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
    }
}