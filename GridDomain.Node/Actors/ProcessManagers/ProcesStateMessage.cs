namespace GridDomain.Node.Actors.ProcessManagers
{
    public class ProcesStateMessage<T>
    {
        public T State { get; }

        public ProcesStateMessage(T state)
        {
            State = state;
        }
    }
}