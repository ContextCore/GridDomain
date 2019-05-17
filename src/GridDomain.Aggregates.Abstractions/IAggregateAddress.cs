namespace GridDomain.Aggregates.Abstractions
{
    /// <summary>
    /// Represents an aggregate instance location in the abstract aggregate pool inside the domain
    /// </summary>
    public interface IAggregateAddress
    {
        /// <summary>
        /// Name of the aggregate kind, usually it is a name of the corresponding C# class representing the aggregate.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// An aggregate instance identifier inside its kind. Different kinds can reuse the same Id.
        /// It is recommended to take business identifiers instead of artificial ones like GUID
        /// </summary>
        string Id { get; }
    }
}