using Automatonymous;

namespace GridDomain.Node.Actors.EventSourced.SnapshotsPolicy {
    public class OperationStateMachine<T> :  AutomatonymousStateMachine<OperationState<T>> where T : class
    {
        public OperationStateMachine()
        {
            
            During(Initial,
                   When(Start)
                       .Then(c => c.Instance.Data = c.Data)
                       .TransitionTo(Started));

            During(Started,
                   When(Fail)
                       .Then(c => c.Instance.IsCompleted = true)
                       .TransitionTo(Failed));
            
            During(Started,
                   When(Complete)
                       .Then(c => c.Instance.IsCompleted = true)
                       .TransitionTo(Completed));

        }
        
        public Event<T> Start { get; private set; }
        public Event<T> Complete { get; private set; }
        public Event<T> Fail { get; private set; }

        public State Started { get; private set; }
        public State Completed { get; private set; }
        public State Failed { get; private set; }
    }
}