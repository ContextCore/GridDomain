namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiterProducer
    {
        IMessagesWaiterBuilder<IMessageWaiter> NewWaiter();
        IMessagesWaiterBuilder<ICommandWaiter> NewCommandWaiter();
    }
}