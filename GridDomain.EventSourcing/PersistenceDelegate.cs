using System.Threading.Tasks;

namespace GridDomain.EventSourcing {
    public delegate Task PersistenceDelegate(Aggregate evt);
}