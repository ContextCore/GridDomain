using System.Threading.Tasks;

namespace GridDomain.Aggregates.Abstractions {
    public interface IAggregatesDomainConfiguration
    {
        Task Register(IAggregatesDomainBuilder builder);
    }
    
}