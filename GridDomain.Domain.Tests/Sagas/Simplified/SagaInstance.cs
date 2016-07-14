using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using NEventStore;

namespace GridDomain.Tests.Sagas.Simplified
{

    public class SagaInstance<TSagaInstance>: ISagaInstance where TSagaInstance : class, ISagaProgress
    {
        public readonly SagaStateMachine<TSagaInstance> Machine;
        public readonly TSagaInstance Progress;
        public IReadOnlyCollection<object> CommandsToDispatch => Machine.CommandsToDispatch;
        public void ClearCommandsToDispatch()
        {
            Machine.CommandsToDispatch.Clear();
        }

        public IAggregate State { get; }

        public SagaInstance(SagaStateMachine<TSagaInstance> machine, TSagaInstance progress, State initialState)
        {
            Progress = progress;
            Machine = machine;
            Machine.TransitionToState(progress, initialState);
        }

        public void Transit(object message)
        {
            var messageType = message.GetType();
            var method = typeof (SagaInstance<TSagaInstance>)
                            .GetMethod(nameof(Transit))
                            .MakeGenericMethod(messageType);

            method.Invoke(this,new [] {message});
        }

        public void Transit<TMessage>(TMessage message) where TMessage : class
        {
            Machine.RaiseByExternalEvent(Progress, message);
        }
    }
}
