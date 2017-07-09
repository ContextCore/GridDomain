namespace GridDomain.Node.Actors.Sagas.Messages
{
    internal class NotifyOnSagaTransitedAck
    {
        private NotifyOnSagaTransitedAck() {}

        public static readonly NotifyOnSagaTransitedAck Instance = new NotifyOnSagaTransitedAck();
    }
}