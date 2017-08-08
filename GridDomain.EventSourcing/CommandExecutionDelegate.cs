using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing {
    public delegate Task<TAggregate> CommandExecutionDelegate<TAggregate>(TAggregate agr, ICommand cmd, PersistenceDelegate persistenceDelegate) where TAggregate : Aggregate;
}