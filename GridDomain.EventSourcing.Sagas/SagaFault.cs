using System;
using CommonDomain;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFault<out TState> : ISagaFault
                                    where TState : IAggregate
    {
        new TState State { get; }
    }

    public class SagaFault<TState> : ISagaFault<TState> where TState : IAggregate
    {
        private SagaFault(Guid sagaId, ICommandFault commandFault, TState state)
        {
            SagaId = sagaId;
            CommandFault = commandFault;
            State = state;
        }

        public SagaFault(IDomainStateSaga<TState> saga, ICommandFault fault):this(saga.State.Id, fault, saga.State)
        {
            
        }

        public Guid SagaId { get; }

        public ICommandFault CommandFault { get; }
        object ISagaFault.State => State;

        public TState State { get; }
    }
}