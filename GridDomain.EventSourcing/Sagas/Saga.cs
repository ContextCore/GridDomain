using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;
using Serilog;

namespace GridDomain.EventSourcing.Sagas
{
    public class Saga<TState> : ISaga<TState> where TState : class, ISagaState
    {
        private readonly ILogger _log;
        public readonly Process<TState> Machine;

        public Saga(Process<TState> machine,
                    TState state,
                    ILogger log,
                    bool doUninitializedWarnings = true)
        {
            if (machine == null)
                throw new ArgumentNullException(nameof(machine));
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            _log = log;
            Machine = machine;
            State = state;
            CheckInitialState(doUninitializedWarnings);
        }

        private string CurrentStateName => State.CurrentStateName;

        public TState State { get; private set; }
        
        /// <summary>
        /// Transit saga to new state is possible only after state persistence
        /// So we need to 'preview' it and save after due to mismathings in autonomyous \ akka persistence mechanics
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<TransitionResult<TState>> PreviewTransit<TMessage>(TMessage message) where TMessage : class
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
                                                throw new SagaTransitionException(message, t.Exception);
                                           
                                            return new TransitionResult<TState>(newState,commandsToDispatch);
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

        public void ApplyTransit(TState newState)
        {
            State = newState;
        }
    }
}