using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Aggregates
{
    public static class DomainEventExtensions
    {
        public static Task<IReadOnlyCollection<IDomainEvent>> AsCommandResult(this IDomainEvent e)
        {
            return Task.FromResult((IReadOnlyCollection<IDomainEvent>)new [] {e});
        }
        
        public static Task<IReadOnlyCollection<IDomainEvent>> AsCommandResult(this IDomainEvent[] e)
        {
            return Task.FromResult((IReadOnlyCollection<IDomainEvent>)e);
        }
    }
}