using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.ProcessManagers {
    public class ConventionProcess<TState> : Process<TState>, IProcessDescriptor where TState : class, IProcessState
    {
        private readonly ConventionProcessMessageRouter<TState> _transitionRouter;
        private readonly IProcessDescriptor _descriptor;
        protected ConventionProcess()
        {
            var processType = GetType();
            _transitionRouter = new ConventionProcessMessageRouter<TState>(processType);
            _descriptor = ProcessDescriptor.ScanByConvention(processType);
        }

        public override Task<IReadOnlyCollection<ICommand>> Transit(TState state, object message)
        {
            return _transitionRouter.Dispatch(this, state, message);
        }

        public IReadOnlyCollection<MessageBind> AcceptMessages => _descriptor.AcceptMessages;

        public Type StateType => _descriptor.StateType;

        public Type ProcessType => _descriptor.ProcessType;
    }
}