namespace GridDomain.Node.Actors
{
    class SagaState<T>
    {
        public T State { get; }

        public SagaState(T state)
        {
            State = state;
        }
    }
}