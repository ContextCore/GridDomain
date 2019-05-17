namespace GridDomain.Aggregates
{
    public interface ICommand : IHaveId
    {
        IAggregateAddress Recipient { get; }
    }
}