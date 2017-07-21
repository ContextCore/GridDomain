using System;

namespace GridDomain.Processes
{
    public class ProcessTransitionException : Exception
    {
        public ProcessTransitionException(object transitionMessage, Exception inner)
            : base("Saga transition raised an error", inner)
        {
            TransitionMessage = transitionMessage;
        }

        public object TransitionMessage { get; }
    }
}