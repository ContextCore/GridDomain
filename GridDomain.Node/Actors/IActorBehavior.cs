using System.Threading.Tasks;

namespace GridDomain.Node.Actors {
    public interface IActorBehavior
    {
        IActorState State { get; }
        void Enter();
        Task<IActorBehavior> Execute();
    }
}