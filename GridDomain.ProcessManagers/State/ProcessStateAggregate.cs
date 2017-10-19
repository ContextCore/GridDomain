using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.ProcessManagers.State
{
    public class ProcessStateAggregate<TState> : CommandAggregate where TState : IProcessState
    {
        public ProcessStateAggregate(TState state): this(state.Id)
        {
            Condition.NotNull(() => state);
            Produce(new ProcessManagerCreated<TState>(state, state.Id));
        }
        
        private ProcessStateAggregate(Guid id) : base(id)
        {
        }

        public TState State { get; private set; }

        public void ReceiveMessage(TState state, Guid messageId)
        {
            Produce(new ProcessReceivedMessage<TState>(Id, state, messageId));
        }

        protected override void OnAppyEvent(DomainEvent evt)
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

        private static readonly Type[] KnownCommands = {typeof(SaveStateCommand<TState>), typeof(CreateNewStateCommand<TState>)};
        public override IReadOnlyCollection<Type> RegisteredCommands => KnownCommands;
        protected override Task<IAggregate> Execute(ICommand cmd)
        {
            switch (cmd)
            {
                case SaveStateCommand<TState> c:
                    ReceiveMessage(c.State, c.MessageId);
                    break;
                case CreateNewStateCommand<TState> c:
                    return Task.FromResult<IAggregate>(new ProcessStateAggregate<TState>(c.State));
            }
            return Task.FromResult<IAggregate>(this);
        }
    }
}