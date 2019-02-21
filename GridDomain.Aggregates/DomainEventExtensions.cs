using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridDomain.Aggregates
{
    public static class DomainEventExtensions
    {
        public static Task<IReadOnlyCollection<DomainEvent>> AsCommandResult(this DomainEvent e)
        {
            return Task.FromResult((IReadOnlyCollection<DomainEvent>)new [] {e});
        }
    }
}