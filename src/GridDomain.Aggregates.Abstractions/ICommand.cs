namespace GridDomain.Aggregates.Abstractions
{
    public interface ICommand : IHaveId
    {
        IAggregateAddress Recipient { get; }
    }
}