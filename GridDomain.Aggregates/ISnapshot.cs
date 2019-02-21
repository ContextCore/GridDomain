namespace GridDomain.Aggregates
{
    public interface ISnapshot
    {
        string Id { get; set; }

        int Version { get; set; }
    }
}