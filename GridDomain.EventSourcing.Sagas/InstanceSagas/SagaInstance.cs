using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Automatonymous;
using CommonDomain;
using GridDomain.CQRS;
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

    public class SagaInstance<TSaga, TSagaData> : ISagaInstance<TSaga, TSagaData>
        where TSaga : Saga<TSagaData>
        where TSagaData : class, ISagaState
    {
        public readonly Saga<TSagaData> Machine;
        private readonly SagaDataAggregate<TSagaData> _dataAggregate;
        private readonly MethodInfo _transitGenericMethodInfo = typeof(SagaInstance<TSaga, TSagaData>)
                                                                .GetMethods()
                                                                .Single(m => m.IsGenericMethod && m.Name == nameof(Transit));

        private static readonly ILogger Log = LogManager.GetLogger();
        
        private List<ICommand> _commandsToDispatch = new List<ICommand>();
        public IReadOnlyCollection<ICommand> CommandsToDispatch => _commandsToDispatch;
        public void ClearCommandsToDispatch()
        {
            Machine.ClearCommands();
            _commandsToDispatch.Clear();
        }

        SagaDataAggregate<TSagaData> ISagaInstance<TSaga, TSagaData>.Data => _dataAggregate;

        public IAggregate Data => _dataAggregate;

        public SagaInstance(Saga<TSagaData> machine, 
                            SagaDataAggregate<TSagaData> dataAggregate, 
                            bool doUninitializedWarnings = true)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));
            if (dataAggregate == null) throw new ArgumentNullException(nameof(dataAggregate));
            _dataAggregate = dataAggregate;
            Machine = machine;

            if (!CheckInitialState(dataAggregate, doUninitializedWarnings)) return;

            RegisterPersistence(dataAggregate);
        }

        private void RegisterPersistence(SagaDataAggregate<TSagaData> dataAggregate)
        {
            Machine.TransitionToState(_dataAggregate.Data, Machine.GetState(CurrentStateName));
            Machine.OnStateEnter += (sender, context) => dataAggregate.RememberTransition(context.Instance);
            Machine.OnEventReceived += (sender, context) => dataAggregate.RememberEvent(context.Event, context.SagaData, context.EventData);
        }

        private string CurrentStateName => _dataAggregate.Data?.CurrentStateName;

        private bool CheckInitialState(SagaDataAggregate<TSagaData> dataAggregate, bool logUninitializedState = true)
        {
            if (!string.IsNullOrEmpty(CurrentStateName)) return true;

            Log.Warn("Started saga {Type} {Id} without initialization.", GetType().Name, dataAggregate.Id);
            Log.Warn(_dataAggregate.Data == null ? "Saga data is empty" : "Current state name is not specified");

            if (!logUninitializedState) return false;

            Log.Warn("Saga will not process and only record incoming messages");
            Machine.OnMessageReceived += (sender, context) =>
            {
                var msg = context.Message;
                Log.Warn("Not initialized saga {Type} received a message {@Msg}", GetType().Name, msg);
            };
            return false;
        }

        public void Transit(object message)
        {
            if(message == null)
                throw new NullMessageTransitException(_dataAggregate.Data);

            var messageType = message.GetType();

            var method = _transitGenericMethodInfo.MakeGenericMethod(messageType);

            try
            {
                method.Invoke(this, new[] {message});
            }
            catch (TargetInvocationException e)
            {
                //catch special exception and rethrow, otherwise caller cannot handle it
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
        }

        public void Transit<TMessage>(TMessage message) where TMessage : class
        {
            //Saga is not initialized
            if (_dataAggregate.Id == Guid.Empty)
            {
                Log.Trace("Saga {Saga} id is empty and it received message {Message}", typeof(TSaga).Name,message);
                return;
            }

            if (_dataAggregate.Data== null)
            {
                Log.Trace("Saga {Saga} data is empty and it received message {Message}", typeof(TSaga).Name,message);
                return;
            }

            Machine.RaiseByMessage(_dataAggregate.Data, message);
            _commandsToDispatch = Machine.CommandsToDispatch.Select(c => c.CloneWithSaga(Data.Id))
                                                            .Cast<ICommand>()
                                                            .ToList();
        }
    }
}
