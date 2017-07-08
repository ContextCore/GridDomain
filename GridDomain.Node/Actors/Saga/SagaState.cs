namespace GridDomain.Node.Actors.Saga
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