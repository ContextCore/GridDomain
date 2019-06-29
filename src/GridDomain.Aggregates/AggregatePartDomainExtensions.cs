using System.Threading.Tasks;
using GridDomain.Abstractions;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Aggregates
{
    public static class AggregatePartDomainExtensions
    {
        public static IAggregatesGateway Aggregates(this IDomain domain)
        {
            return domain.GetPart<IAggregatesGateway>();
        } 
        
        public static async Task<IAggregatesGateway> Aggregates(this Task<IDomain> domain)
        {
            return (await domain).GetPart<IAggregatesGateway>();
        }
    }
}