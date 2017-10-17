using System;
using System.Linq.Expressions;
using Automatonymous;
using GridDomain.Common;

namespace GridDomain.ProcessManagers.DomainBind {
    public class ProcessDescriptor<TProcess, TState> : ProcessDescriptor where TProcess : Process<TState>
                                                                                       where TState : class, IProcessState

    {
        public ProcessDescriptor()
            : base(typeof(TState), typeof(TProcess)) {}

        public void MapDomainEvent<TDomainEvent>(Expression<Func<TProcess, Event<TDomainEvent>>> machineEvent,
                                                 Expression<Func<TDomainEvent, Guid>> correlationFieldExpression)
        {
            AddAcceptedMessage(typeof(TDomainEvent), MemberNameExtractor.GetName(correlationFieldExpression));
        }
    }
}