namespace GridDomain.Aggregates.Abstractions
{
    public interface ISnapshot
    {
        string Id { get; set; }

        int Version { get; set; }
    }
}