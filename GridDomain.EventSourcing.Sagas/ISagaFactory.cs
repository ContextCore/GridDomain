namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFactory<out TSaga, in TMessage>
    {
        TSaga Create(TMessage message);
    }
}