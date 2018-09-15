using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.ProcessManagers.State
{
    public class ProcessStateAggregate<TState> : Aggregate where TState : IProcessState
    {
        
        public ProcessStateAggregate(TState state): this(state.Id)
        {
            Condition.NotNull(() => state);
            Emit(new ProcessManagerCreated<TState>(state, state.Id));
        }
        
        private ProcessStateAggregate(string id) : base(id)
        {
        }

        public TState State { get;  private set; }

        public void ReceiveMessage(TState state, string messageId)
        {
            Emit(new ProcessReceivedMessage<TState>(Id, state, messageId));
        }

        protected override void OnApplyEvent(DomainEvent evt)
        {
            switch(evt)
            {
                case ProcessReceivedMessage<TState> e:
                    State = e.State;
                    break;
                case ProcessManagerCreated<TState> e:
                    State = e.State;
                    Id = e.SourceId;
                    break;
            }
        }

        public override Task<IReadOnlyCollection<DomainEvent>> Execute(ICommand command)
        {
            _uncommittedEvents.Clear();
            switch (command)
            {
                case SaveStateCommand<TState> c:
                    ReceiveMessage(c.State, c.MessageId);
                    return Task.FromResult<IReadOnlyCollection<DomainEvent>>(_uncommittedEvents);
                case CreateNewStateCommand<TState> c:
                    
                    Emit(new ProcessManagerCreated<TState>(c.State, c.State.Id));
                    return Task.FromResult<IReadOnlyCollection<DomainEvent>>(_uncommittedEvents);
                default:
                    throw new MissingRegisteredCommandsException();
            }
        }

    }
}