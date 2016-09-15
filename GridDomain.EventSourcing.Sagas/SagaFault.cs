using System;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas.StateSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public class SagaFault<TState> : ISagaFault<TState> where TState : IAggregate
    {
        private SagaFault(Guid sagaId, IFault commandFault, TState state)
        {
            SagaId = sagaId;
            CommandFault = commandFault;
            State = state;
        }

        public SagaFault(IDomainStateSaga<TState> saga, IFault fault):this(saga.State.Id, fault, saga.State)
        {
            
        }

        public Guid SagaId { get; }

        public IFault CommandFault { get; }
        object ISagaFault.State => State;

        public TState State { get; }
    }
}