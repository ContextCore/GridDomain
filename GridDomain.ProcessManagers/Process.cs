using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Events;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.ProcessManagers
{
    public abstract class Process<TState> : AutomatonymousStateMachine<TState>, IProcess<TState> where TState : class, IProcessState
    {
        protected Action<Command> Dispatch { get; private set; }
        protected Process()
        {
            InstanceState(d => d.CurrentStateName);
        }
       
        protected Task<ProcessResult<TState>> TransitMessage<TMessage>(Event<TMessage> evt, TMessage message, IProcessState state)
        {
            //TODO: find more performant variant
            var newState = (TState)state.Clone();
            var commandsToDispatch = new List<Command>();
            Dispatch = c =>
                               {
                                   c.ProcessId = state.Id;
                                   commandsToDispatch.Add(c);
                               };

            return this.RaiseEvent(newState, evt, message)
                                    .ContinueWith(t =>
                                                  {
                                                      if(t.IsFaulted)
                                                          throw new ProcessTransitionException(message, t.Exception);

                                                      return new ProcessResult<TState>(newState, commandsToDispatch);
                                                  });
        }

        public abstract Task<ProcessResult<TState>> Transit(TState state, object message);
    }
}