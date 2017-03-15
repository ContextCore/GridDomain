using Automatonymous;
using CommonDomain;
using GridDomain.CQRS;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public static class Saga
    {
        public static Saga<TSaga, TSagaData> New<TSaga, TSagaData>(TSaga saga,
                                                                   SagaStateAggregate<TSagaData> data,
                                                                   ILogger log) where TSaga : SagaStateMachine<TSagaData>
                                                                                where TSagaData : class, ISagaState
        {
            return new Saga<TSaga, TSagaData>(saga,data.Data, log);
        }
    }

    public class Saga<TMachine, TState> : ISaga<TMachine, TState> where TMachine : SagaStateMachine<TState>
                                                                  where TState : class, ISagaState
    {
        private readonly ILogger _log;
        public readonly SagaStateMachine<TState> Machine;

        private readonly List<ICommand> _commandsToDispatch = new List<ICommand>();

        public Saga(SagaStateMachine<TState> machine,
                    TState state,
                    ILogger log,
                    bool doUninitializedWarnings = true)
        {
            if (machine == null)
                throw new ArgumentNullException(nameof(machine));
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            _log = log.ForContext<Saga<TMachine, TState>>();
            Machine = machine;
            Machine.DispatchCallback = c => _commandsToDispatch.Add(c.CloneWithSaga(State.Id));
            State = state;
            CheckInitialState(doUninitializedWarnings);
        }

        private string CurrentStateName => State.CurrentStateName;
        public IReadOnlyCollection<ICommand> CommandsToDispatch => _commandsToDispatch;

        public void ClearCommandsToDispatch()
        {
            _commandsToDispatch.Clear();
        }

        ISagaState ISaga.State => State;
        public TState State { get; set; }

        public Task<ISagaState> CreateNextState<TMessage>(TMessage message) where TMessage : class
        {
            var machineEvent = Machine.GetMachineEvent(message);
            //TODO: find more performant variant
            var newState = (TState)State.Clone();
            return Machine.RaiseEvent(newState, machineEvent, message)
                          .ContinueWith(t =>
                                        {
                                            if (t.IsFaulted)
                                                throw new SagaTransitionException(message, State, t.Exception);
                                           
                                            return (ISagaState)newState;
                                        });
        }

        private bool CheckInitialState(bool logUninitializedState = true)
        {
            if (!string.IsNullOrEmpty(CurrentStateName))
                return true;

            _log.Warning("Started saga {Type} {Id} without initialization.", GetType().Name, State.Id);
            _log.Warning(State == null ? "Saga data is empty" : "Current state name is not specified");

            if (!logUninitializedState)
                return false;

            _log.Warning("Saga will not process and only record incoming messages");
            return false;
        }

        Task<TState> ISaga<TMachine, TState>.CreateNextState<T>(T message)
        {
            return CreateNextState(message).ContinueWith(t => (TState) t.Result);
        }
    }
}