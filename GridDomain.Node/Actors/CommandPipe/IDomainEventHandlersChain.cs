using System.Threading.Tasks;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe
{
    public interface IDomainEventHandlersChain
    {
        Task Process(DomainEvent[] evt);
    }
}