using System;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    class SagaCreatorAdapter<TState> : ISagaCreator<TState, object> where TState : ISagaState
    {
        private readonly Func<object, Guid?, ISaga<TState>> _factory;
        private readonly Type _messageType;

        public SagaCreatorAdapter(Type messageType, Func<object, Guid?, ISaga<TState>> factory)
        {
            _messageType = messageType;
            this._factory = factory;
        }
        public ISaga<TState> CreateNew(object message, Guid? id = null)
        {
            if (message.GetType() != _messageType)
                throw new SagaStartMessageTypeMismatchException();

            return _factory.Invoke(message, id);
        }
    }
}