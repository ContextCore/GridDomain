using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using Stateless;

namespace GridDomain.CQRS.Messaging.MessageRouting.Sagas
{
    class StatefullSaga<TState,TTransition> : SagaBase<object>, IStatefullSaga where TState : struct where TTransition : struct
    {
        public Guid Id => State.Id;
        public IAggregate State { get; }
        protected StateMachine<TState, TTransition> Machine;
        
        public StatefullSaga(SagaStateAggregate<TState, TTransition> state)
        {
            State = state;
            Machine = new StateMachine<TState, TTransition>(state.InitialState);
        }

        public void Transit(TTransition transition)
        {
           // if(!Machine.CanFire(transition))

           // Machine.Fire(transition,);
        }

    }
}
