namespace GridDomain.Node.Actors.Saga.Messages
{
    internal class NotifyOnSagaTransitedAck
    {
        private NotifyOnSagaTransitedAck() {}

        public static readonly NotifyOnSagaTransitedAck Instance = new NotifyOnSagaTransitedAck();
    }
}