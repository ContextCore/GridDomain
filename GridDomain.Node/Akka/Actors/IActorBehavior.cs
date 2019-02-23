using System.Threading.Tasks;

namespace GridDomain.Node.Akka.Actors {
    public interface IActorBehavior
    {
        IActorState State { get; }
        void Enter();
        Task<IActorBehavior> Execute();
    }
}