namespace GridDomain.Node.Configuration.Composition {
    public interface IDomainConfiguration
    {
        void Register(IDomainBuilder builder);
    }
}