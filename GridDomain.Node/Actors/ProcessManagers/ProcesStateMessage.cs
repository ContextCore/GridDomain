namespace GridDomain.Node.Actors.ProcessManagers
{
    class ProcesStateMessage<T>
    {
        public T State { get; }

        public ProcesStateMessage(T state)
        {
            State = state;
        }
    }
}