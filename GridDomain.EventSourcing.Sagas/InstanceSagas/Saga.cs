using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Automatonymous;
using CommonDomain;
using GridDomain.CQRS;
using Serilog;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public static class Saga
    {
        public static Saga<TSaga, TSagaData> New<TSaga, TSagaData>(TSaga saga,
                                                                           SagaStateAggregate<TSagaData> data,
                                                                           ILogger log) where TSaga : SagaStateMachine<TSagaData>
                                                                                        where TSagaData : class, ISagaState
        {
            return new Saga<TSaga, TSagaData>(saga, data, log);
        }
    }

    public class Saga<TSaga, TSagaData> : ISaga<TSaga, TSagaData> where TSaga : SagaStateMachine<TSagaData>
                                                                                  where TSagaData : class, ISagaState
    {
        private readonly SagaStateAggregate<TSagaData> _dataAggregate;

        private readonly ILogger _log;
        public readonly SagaStateMachine<TSagaData> Machine;

        private List<ICommand> _commandsToDispatch = new List<ICommand>();

        public Saga(SagaStateMachine<TSagaData> machine,
                            SagaStateAggregate<TSagaData> dataAggregate,
                            ILogger log,
                            bool doUninitializedWarnings = true)
        {
            if (machine == null)
                throw new ArgumentNullException(nameof(machine));
            if (dataAggregate == null)
                throw new ArgumentNullException(nameof(dataAggregate));
            _dataAggregate = dataAggregate;
            _log = log.ForContext<Saga<TSaga, TSagaData>>();
            Machine = machine;

            CheckInitialState(dataAggregate, doUninitializedWarnings);
        }

        private string CurrentStateName => _dataAggregate.Data?.CurrentStateName;
        public IReadOnlyCollection<ICommand> CommandsToDispatch => _commandsToDispatch;

        public void ClearCommandsToDispatch()
        {
            Machine.ClearCommands();
            _commandsToDispatch.Clear();
        }

        SagaStateAggregate<TSagaData> ISaga<TSaga, TSagaData>.Data => _dataAggregate;

        public IAggregate Data => _dataAggregate;

        public Task Transit<TMessage>(TMessage message) where TMessage : class
        {
            //Saga is not initialized
            if (_dataAggregate.Id == Guid.Empty)
            {
                _log.Verbose("Saga {Saga} id is empty and it received message {Message}", typeof(TSaga).Name, message);
                return Task.CompletedTask;
            }

            if (_dataAggregate.Data == null)
            {
                _log.Verbose("Saga {Saga} data is empty and it received message {Message}", typeof(TSaga).Name, message);
                return Task.CompletedTask;
            }

            var machineEvent = Machine.GetMachineEvent(message);

            return Machine.RaiseEvent(_dataAggregate.Data, machineEvent, message)
                          .ContinueWith(t =>
                                        {
                                            if (t.IsFaulted)
                                                throw new SagaTransitionException(message, _dataAggregate.Data, t.Exception);

                                            _dataAggregate.RememberEvent(_dataAggregate.Data, message, machineEvent.Name);
                                            _commandsToDispatch = Machine.CommandsToDispatch.Select(c => c.CloneWithSaga(Data.Id)).Cast<ICommand>().ToList();
                                        });
        }

        private bool CheckInitialState(SagaStateAggregate<TSagaData> dataAggregate, bool logUninitializedState = true)
        {
            if (!string.IsNullOrEmpty(CurrentStateName))
                return true;

            _log.Warning("Started saga {Type} {Id} without initialization.", GetType().Name, dataAggregate.Id);
            _log.Warning(_dataAggregate.Data == null ? "Saga data is empty" : "Current state name is not specified");

            if (!logUninitializedState)
                return false;

            _log.Warning("Saga will not process and only record incoming messages");
            return false;
        }
    }
}