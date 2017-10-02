using System;

namespace GridDomain.ProcessManagers
{
    public class ProcessTransitionException : Exception
    {
        public ProcessTransitionException()
        {
            
        }
        public ProcessTransitionException(object transitionMessage, Exception inner)
            : base("Process transition raised an error", inner)
        {
            TransitionMessage = transitionMessage;
        }

        public object TransitionMessage { get; }
    }
}