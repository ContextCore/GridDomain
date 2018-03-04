using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing {
    public delegate Task<TAggregate> CommandExecutionDelegate<TAggregate>(TAggregate agr, ICommand cmd) where TAggregate : IAggregate;
}