using System.Collections.Generic;
using Automatonymous;
using CommonDomain;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{

    public static class SagaInstance
    {
        public static SagaInstance<TSaga, TSagaData> New<TSaga, TSagaData>(TSaga saga, SagaDataAggregate<TSagaData> data) 
            where TSaga : Saga<TSagaData> 
            where TSagaData : class, ISagaState<State>
        {
            return new SagaInstance<TSaga, TSagaData>(saga, data);
        }
    }

    public class SagaInstance<TSaga,TSagaData>: ISagaInstance<TSaga, TSagaData> 
        where TSaga : Saga<TSagaData>
        where TSagaData : class, ISagaState<State>
    {
        public readonly Saga<TSagaData> Machine;
        private readonly SagaDataAggregate<TSagaData> _stateStorage;
        public IReadOnlyCollection<object> CommandsToDispatch => Machine.CommandsToDispatch;
        public void ClearCommandsToDispatch()
        {
            Machine.CommandsToDispatch.Clear();
        }

        SagaDataAggregate<TSagaData> ISagaInstance<TSaga, TSagaData>.Data => _stateStorage;

        public IAggregate Data => _stateStorage;

        public SagaInstance(Saga<TSagaData> machine, SagaDataAggregate<TSagaData> stateStorage)
        {
            _stateStorage = stateStorage;
            Machine = machine;
            Machine.TransitionToState(_stateStorage.Data, _stateStorage.Data.CurrentState); 
            Machine.OnStateEnter += (sender, context) => stateStorage.RememberTransition(context.State, context.Instance);
            Machine.OnEventReceived += (sender, context) => stateStorage.RememberEvent(context.Event, context.SagaData, context.EventData);
        }

      

        public void Transit(object message)
        {
            var messageType = message.GetType();
            var method = this.GetType()
                             .GetMethod(nameof(Transit))
                             .MakeGenericMethod(messageType);

            method.Invoke(this,new [] {message});
        }

        public void Transit<TMessage>(TMessage message) where TMessage : class
        {
            Machine.RaiseByExternalEvent(_stateStorage.Data, message);
        }
    }
}
