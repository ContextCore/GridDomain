using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridDomain.Aggregates
{
    public static class DomainEventExtensions
    {
        public static Task<IReadOnlyCollection<IDomainEvent>> AsCommandResult(this IDomainEvent e)
        {
            return Task.FromResult((IReadOnlyCollection<IDomainEvent>)new [] {e});
        }
    }
}