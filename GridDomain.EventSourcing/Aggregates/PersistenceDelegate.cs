using System.Threading.Tasks;

namespace GridDomain.EventSourcing.Aggregates {
    public delegate Task PersistenceDelegate(Aggregate evt);
}