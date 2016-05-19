using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using Stateless;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    public abstract class StateSaga<TState,TTransition>: SagaBase<object> where TTransition : struct where TState : struct
    {
        private SagaStateAggregate<TState, TTransition> StateDetails;
        protected object LastEvent;
        protected StateMachine<TState, TTransition> Machine;

        protected abstract void InitializeMachine(StateMachine<TState, TTransition> machine);

        public void Initialize()
        {
            Machine = new StateMachine<TState, TTransition>(StateDetails.MachineState);
            InitializeMachine(Machine);
        }

        public void Transit(TTransition transition)
        {
            if (!Machine.CanFire(transition)) return;

            Machine.Fire(transition);
            StateDetails.StateChanged(transition,Machine.State);
        }

        public StateSaga(Guid id, SagaStateAggregate<TState, TTransition> state)//:base(id)
        {
            StateDetails = state;
        }

        public void IssueCommand(ICommand command)
        {
            this.Dispatch(command);
        }

        public void Consume(object @event)
        {
            
        }

    }
}
