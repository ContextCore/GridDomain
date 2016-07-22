using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private readonly SagaDataAggregate<TSagaData> _dataAggregate;
        private readonly MethodInfo _transitGenericMethodInfo;

        public IReadOnlyCollection<object> CommandsToDispatch => Machine.CommandsToDispatch;
        public void ClearCommandsToDispatch()
        {
            Machine.CommandsToDispatch.Clear();
        }

        SagaDataAggregate<TSagaData> ISagaInstance<TSaga, TSagaData>.Data => _dataAggregate;

        public IAggregate Data => _dataAggregate;

        public SagaInstance(Saga<TSagaData> machine, SagaDataAggregate<TSagaData> dataAggregate)
        {
            _dataAggregate = dataAggregate;
            Machine = machine;

            var currentStateName = dataAggregate.CurrentStateName;
            var initialState = Machine.GetState(currentStateName);
            Machine.TransitionToState(dataAggregate.Data, initialState);

            Machine.OnStateEnter += (sender, context) => dataAggregate.RememberTransition(context.State.Name, context.Instance);
            Machine.OnEventReceived += (sender, context) => dataAggregate.RememberEvent(context.Event, context.SagaData, context.EventData);
            _transitGenericMethodInfo = GetType()
                                       .GetMethods()
                                       .Single(m => m.IsGenericMethod && m.Name == nameof(Transit));
        }
        
        public void Transit(object message)
        {
            var messageType = message.GetType();
            
            var method = _transitGenericMethodInfo.MakeGenericMethod(messageType);

            method.Invoke(this,new [] {message});
        }

        public void Transit<TMessage>(TMessage message) where TMessage : class
        {
            Machine.RaiseByExternalEvent(_dataAggregate.Data, message);
        }
    }
}
