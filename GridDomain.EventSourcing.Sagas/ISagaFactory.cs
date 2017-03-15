namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFactory<out TSaga, in TMessage> where TSaga : ISaga
    {
        TSaga Create(TMessage message);
    }
}