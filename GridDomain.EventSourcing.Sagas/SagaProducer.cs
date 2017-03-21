using System;
using System.Collections.Generic;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaProducer<TState> : ISagaProducer<TState> where TState : ISagaState
    {
        private readonly Dictionary<Type, Func<object, ISaga<TState>>> _factories = new Dictionary<Type, Func<object, ISaga<TState>>>();

        public SagaProducer(ISagaDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public ISaga<TState> Create(object message)
        {
            var type = message.GetType();
            Func<object, ISaga<TState>> factory;
            if (!_factories.TryGetValue(type, out factory))
                throw new CannotFindFactoryForSagaCreation(typeof(TState), message);

            return factory.Invoke(message);
        }

        public ISagaDescriptor Descriptor { get; }

        public IReadOnlyCollection<Type> KnownDataTypes => _factories.Keys;

        public void RegisterAll<TFactory>(TFactory factory) where TFactory : IFactory<ISaga<TState>, TState>
        {
            dynamic dynamicfactory = factory;

            foreach (var startMessageType in Descriptor.StartMessages)
            {
                var expectedFactoryType = typeof(IFactory<,>).MakeGenericType(typeof(ISaga<TState>), startMessageType);
                if (!expectedFactoryType.IsInstanceOfType(factory))
                    throw new FactoryNotSupportStartMessageException(factory.GetType(), startMessageType);

                //TODO: think how to avoid dynamic call and call method need by message
                Func<object, ISaga<TState>> fct = msg => (ISaga<TState>) dynamicfactory.Create((dynamic) msg);
                Register(startMessageType, fct);
            }

            Register(factory);
        }

        public void Register<TMessage>(IFactory<ISaga<TState>, TMessage> factory)
        {
            Register(typeof(TMessage), m => factory.Create((TMessage) m));
        }

        private void Register(Type dataType, Func<object, ISaga<TState>> factory)
        {
            if (_factories.ContainsKey(dataType))
                throw new FactoryAlreadyRegisteredException(dataType);

            _factories[dataType] = factory.Invoke;
        }
    }
}