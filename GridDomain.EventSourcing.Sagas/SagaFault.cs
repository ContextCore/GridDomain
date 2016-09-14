using System;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.StateSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaFault<TState> : ISagaFault<TState> where TState : IAggregate
    {
        private SagaFault(Guid sagaId, IMessageFault commandFault, TState state)
        {
            SagaId = sagaId;
            CommandFault = commandFault;
            State = state;
        }

        public SagaFault(IDomainStateSaga<TState> saga, IMessageFault fault):this(saga.State.Id, fault, saga.State)
        {
            
        }

        public Guid SagaId { get; }

        public IMessageFault CommandFault { get; }
        object ISagaFault.State => State;

        public TState State { get; }
    }
}