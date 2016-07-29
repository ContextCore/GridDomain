using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Automatonymous;
using CommonDomain;
using GridDomain.Logging;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{

    public static class SagaInstance
    {
        public static SagaInstance<TSaga, TSagaData> New<TSaga, TSagaData>(TSaga saga, SagaDataAggregate<TSagaData> data) 
            where TSaga : Saga<TSagaData> 
            where TSagaData : class, ISagaState
        {
            return new SagaInstance<TSaga, TSagaData>(saga, data);
        }
    }

    public class SagaInstance<TSaga,TSagaData>: ISagaInstance<TSaga, TSagaData> 
        where TSaga : Saga<TSagaData>
        where TSagaData : class, ISagaState
    {
        public readonly Saga<TSagaData> Machine;
        private readonly SagaDataAggregate<TSagaData> _dataAggregate;
        private readonly MethodInfo _transitGenericMethodInfo;
        private ISoloLogger _log = LogManager.GetLogger();

        public IReadOnlyCollection<object> CommandsToDispatch => Machine.CommandsToDispatch;
        public void ClearCommandsToDispatch()
        {
            Machine.CommandsToDispatch.Clear();
        }

        SagaDataAggregate<TSagaData> ISagaInstance<TSaga, TSagaData>.Data => _dataAggregate;

        public IAggregate Data => _dataAggregate;

        public SagaInstance(Saga<TSagaData> machine, SagaDataAggregate<TSagaData> dataAggregate)
        {
            if(machine == null) throw new ArgumentNullException(nameof(machine));
            if(dataAggregate == null) throw new ArgumentNullException(nameof(dataAggregate));
            _dataAggregate = dataAggregate;
            Machine = machine;

            var sagaData = _dataAggregate.Data;
            if (!string.IsNullOrEmpty(sagaData?.CurrentStateName))
            {
                var initialState = Machine.GetState(sagaData.CurrentStateName);
                Machine.TransitionToState(sagaData, initialState);
                Machine.OnMessageReceived += (sender, context) =>
                {
                    var msg = context.Message;
                    _log.Warn("Not initialized saga" + this.GetType().Name +
                              "received a message {@msg}", msg);
                };
            }
            else
            {
                _log.Warn($"Started saga {this.GetType().Name} id = {dataAggregate.Id} without initial state.\r\n" +
                          $"Saga will not process, only record incoming messages");
            }

            Machine.OnStateEnter += (sender, context) => dataAggregate.RememberTransition(context.Instance);
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
            Machine.RaiseByMessage(_dataAggregate.Data, message);
        }
    }
}
