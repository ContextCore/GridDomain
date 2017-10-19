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
       
        protected async Task<IReadOnlyCollection<ICommand>> TransitMessage<TMessage>(Event<TMessage> evt, TMessage message, TState state)
        {
            var commandsToDispatch = new List<Command>();
            Dispatch = c => {
                                c.ProcessId = state.Id;
                                commandsToDispatch.Add(c);
                            };
            try
            {
                await this.RaiseEvent(state, evt, message);
                return commandsToDispatch;
            }
            catch (EventExecutionException ex)
            {
                Exception unwrapped = ex;
                //sometimes eventExecution excpetions are nested
                do
                {
                    unwrapped = unwrapped.InnerException;
                }
                while (unwrapped is EventExecutionException);

                throw new ProcessTransitionException(message, unwrapped);
            }
            catch (Exception ex)
            {
               
                throw new ProcessTransitionException(message, ex);
            }
        }

        public abstract Task<IReadOnlyCollection<ICommand>> Transit(TState state, object message);
    }
}