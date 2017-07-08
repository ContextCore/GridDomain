namespace GridDomain.Node.Actors.Saga.Messages
{
    class GetSagaState
    {
        private GetSagaState() {}

        public static readonly GetSagaState Instance = new GetSagaState();
    }
}