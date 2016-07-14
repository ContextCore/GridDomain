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

    public class SagaInstance<TSagaData>: ISagaInstance where TSagaData : class, ISagaState<State>
    {
        public readonly Saga<TSagaData> Machine;
        public readonly TSagaData Instance;
        public IReadOnlyCollection<object> CommandsToDispatch => Machine.CommandsToDispatch;
        public void ClearCommandsToDispatch()
        {
            Machine.CommandsToDispatch.Clear();
        }

        public IAggregate Data { get; }

        public SagaInstance(Saga<TSagaData> machine, SagaDataAggregate<TSagaData> stateStorage)
        {
            Instance = stateStorage.Data;
            Machine = machine;
            Machine.OnStateEnter += (sender, data) => stateStorage.RememberNewData(data.Event, data.Instance);
            Machine.TransitionToState(Instance, Instance.CurrentState);
        }

        public void Transit(object message)
        {
            var messageType = message.GetType();
            var method = typeof (SagaInstance<TSagaData>)
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
