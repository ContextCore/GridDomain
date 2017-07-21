using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.CQRS;
using Serilog;

namespace GridDomain.Processes
{
    public class ProcessManager<TState> : IProcessManager<TState> where TState : class, IProcessState
    {
        private readonly ILogger _log;
        private Process<TState> Machine { get; }

        public ProcessManager(Process<TState> machine,
                    TState state,
                    ILogger log,
                    bool doUninitializedWarnings = true)
        {
            _log = log;
            Machine = machine ?? throw new ArgumentNullException(nameof(machine));
            State = state ?? throw new ArgumentNullException(nameof(state));
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
        public Task<ProcessResult<TState>> Transit<TMessage>(TMessage message) where TMessage : class
        {
            var machineEvent = Machine.GetMachineEvent(message);

            //TODO: find more performant variant
            var newState = (TState)State.Clone();
            var commandsToDispatch = new List<Command>();
            Machine.DispatchCallback = c => commandsToDispatch.Add(c.CloneForProcess(State.Id));

            return Machine.RaiseEvent(newState, machineEvent, message)
                          .ContinueWith(t =>
                                        {
                                            if (t.IsFaulted)
                                                throw new ProcessTransitionException(message, t.Exception);
                                           
                                            return new ProcessResult<TState>(newState,commandsToDispatch);
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