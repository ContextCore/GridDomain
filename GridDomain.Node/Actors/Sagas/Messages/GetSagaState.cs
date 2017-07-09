namespace GridDomain.Node.Actors.Sagas.Messages
{
    class GetSagaState
    {
        private GetSagaState() {}

        public static readonly GetSagaState Instance = new GetSagaState();
    }
}