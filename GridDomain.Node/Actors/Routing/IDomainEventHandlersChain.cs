using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    public interface IDomainEventHandlersChain
    {
        Task Process(DomainEvent[] evt);
    }
}