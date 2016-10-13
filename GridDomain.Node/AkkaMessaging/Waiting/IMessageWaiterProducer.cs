namespace GridDomain.Node.AkkaMessaging.Waiting
{
    public interface IMessageWaiterProducer
    {
        IMessagesWaiterBuilder<IMessageWaiter> Expect();
        IMessagesWaiterBuilder<ICommandWaiter> ExpectCommand();
    }
}