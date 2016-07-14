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

    public class SagaInstance<TSagaProgress>: ISagaInstance where TSagaProgress : class, ISagaProgress<State>
    {
        public readonly SagaStateMachine<TSagaProgress> Machine;
        public readonly TSagaProgress Instance;
        public IReadOnlyCollection<object> CommandsToDispatch => Machine.CommandsToDispatch;
        public void ClearCommandsToDispatch()
        {
            Machine.CommandsToDispatch.Clear();
        }

        public IAggregate State { get; }

        public SagaInstance(SagaStateMachine<TSagaProgress> machine, TSagaProgress instance)
        {
            Instance = instance;
            Machine = machine;
            Machine.TransitionToState(instance, instance.CurrentState);
        }

        public void Transit(object message)
        {
            var messageType = message.GetType();
            var method = typeof (SagaInstance<TSagaProgress>)
                            .GetMethod(nameof(Transit))
                            .MakeGenericMethod(messageType);

            method.Invoke(this,new [] {message});
        }

        public void Transit<TMessage>(TMessage message) where TMessage : class
        {
            Machine.RaiseByExternalEvent(Instance, message);
        }
    }
}
