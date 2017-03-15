namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFactory<out TSaga, in TMessage> where TSaga : ISagaInstance
    {
        TSaga Create(TMessage message);
    }
}