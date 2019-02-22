using GridDomain.Common;

namespace GridDomain.Node {
    public interface IDomainConfiguration
    {
        void Register(IDomainBuilder builder);
    }

    public interface IDomainConfiguration<TAggregate> :IFor<TAggregate>
    {
        void Register(IDomainBuilder builder);
    }
}