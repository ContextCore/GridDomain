namespace GridDomain.Tests.Sagas.Simplified
{
    public interface ISagaProgress<T>
    {
        T CurrentState { get; set; }
    }
}