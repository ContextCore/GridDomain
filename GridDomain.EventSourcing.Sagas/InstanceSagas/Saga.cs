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

    public class StatePreview<TState>
    {
        public TState State { get; }
        public IReadOnlyCollection<Command> ProducedCommands { get; }

        public StatePreview(TState state, IReadOnlyCollection<Command> producedCommands)
        {
            State = state;
            ProducedCommands = producedCommands;
        }
    }
    public class Saga<TMachine, TState> : ISaga<TMachine, TState> where TMachine : SagaStateMachine<TState>
                                                                  where TState : class, ISagaState
    {
        private readonly ILogger _log;
        public readonly SagaStateMachine<TState> Machine;

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
            State = state;
            CheckInitialState(doUninitializedWarnings);
        }

        private string CurrentStateName => State.CurrentStateName;

        public TState State { get; set; }
        
        /// <summary>
        /// Transit saga to new state is possible only after state persistence
        /// So we need to 'preview' it and save after due to mismathings in autonomyous \ akka persistence mechanics
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<StatePreview<TState>> CreateNextState<TMessage>(TMessage message) where TMessage : class
        {
            var machineEvent = Machine.GetMachineEvent(message);
            

            //TODO: find more performant variant
            var newState = (TState)State.Clone();
            var commandsToDispatch = new List<Command>();
            Machine.DispatchCallback = c => commandsToDispatch.Add(c.CloneWithSaga(State.Id));

            return Machine.RaiseEvent(newState, machineEvent, message)
                          .ContinueWith(t =>
                                        {
                                            if (t.IsFaulted)
                                                throw new SagaTransitionException(message, State, t.Exception);
                                           
                                            return new StatePreview<TState>(newState,commandsToDispatch);
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
    }
}