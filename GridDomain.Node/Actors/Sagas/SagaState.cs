namespace GridDomain.Node.Actors.Sagas
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