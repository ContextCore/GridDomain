using System;
using System.Linq.Expressions;
using Automatonymous;
using GridDomain.Common;

namespace GridDomain.Processes.DomainBind {
    public class ProcessManagerDescriptor<TProcess, TState> : ProcessManagerDescriptor where TProcess : Process<TState>
                                                                                       where TState : class, IProcessState

    {
        public ProcessManagerDescriptor()
            : base(typeof(TState), typeof(TProcess)) {}

        public void MapDomainEvent<TDomainEvent>(Expression<Func<TProcess, Event<TDomainEvent>>> machineEvent,
                                                 Expression<Func<TDomainEvent, Guid>> correlationFieldExpression)
        {
            AddAcceptedMessage(typeof(TDomainEvent), MemberNameExtractor.GetName(correlationFieldExpression));
        }
    }
}