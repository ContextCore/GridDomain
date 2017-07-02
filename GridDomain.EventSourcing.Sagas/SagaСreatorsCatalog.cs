using System;
using System.Reflection;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public class Saga—reatorsCatalog<TState> : TypeCatalog<Func<object, Guid?, ISaga<TState>>, object>,
                                               ISagaCreatorCatalog<TState> where TState : ISagaState
    {
        private readonly ISagaCreator<TState> _fromStateCreator;
        private readonly ISagaDescriptor _descriptor;

        public Saga—reatorsCatalog(ISagaDescriptor descriptor, ISagaCreator<TState> stateCreator)
        {
            _descriptor = descriptor;
            _fromStateCreator = stateCreator;
        }

        private void Register<TMessage>(ISagaCreator<TState, TMessage> factory)
        {
            Add<TMessage>((msg, id) => factory.CreateNew((TMessage) msg, id));
        }

        public ISaga<TState> CreateNew(object startMessage, Guid? id = null)
        {
            var sagaCreator = Get(startMessage);
            if (sagaCreator == null)
                throw new CannotFindFactoryForSagaCreation(typeof(TState), startMessage);

            return sagaCreator.Invoke(startMessage, id);
        }

        public ISaga<TState> Create(TState state)
        {
            return _fromStateCreator.Create(state);
        }

        public void RegisterAll(object factory)
        {
            foreach (var startMessageType in _descriptor.StartMessages)
            {
                var expectedFactoryType = typeof(ISagaCreator<,>).MakeGenericType(typeof(TState), startMessageType);
                if (!expectedFactoryType.IsInstanceOfType(factory))
                    throw new FactoryNotSupportStartMessageException(factory.GetType(), startMessageType);

                var registerMethod = typeof(Saga—reatorsCatalog<TState>)
                    .GetMethod(nameof(Register), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(startMessageType);

                registerMethod.Invoke(this, new []{factory});
            }
        }

        public bool CanCreateFrom(object message)
        {
            return Catalog.ContainsKey(message.GetType());
        }
    }
}