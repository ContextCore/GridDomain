using System;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public class SagaTransitionTimeoutException : Exception
    {
        public string MachineEvent { get; }
        public object Message { get; }

        public SagaTransitionTimeoutException(string machineEvent, object message)
        {
            this.MachineEvent = machineEvent;
            this.Message = message;
        }
    }
}