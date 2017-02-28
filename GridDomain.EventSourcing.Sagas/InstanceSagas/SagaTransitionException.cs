using System;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaTransitionException : Exception
    {
        public SagaTransitionException(object message, ISagaState progress, Exception inner)
            : base("Saga transition raised an error", inner)
        {
            SagaData = progress;
            TransitionMessage = message;
        }

        public object TransitionMessage { get; }
        public ISagaState SagaData { get; }
    }
}