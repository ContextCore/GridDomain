using System.Threading.Tasks;

namespace GridDomain.Node.Akka.Actors {
    public class ActorBehavior<T>: IActorBehavior where T:IActorState
    {
        public T State { get; private set; }

        public ActorBehavior(T initialState)
        {
            State = initialState;
        }
        public void Enter(T state) { }

        IActorState IActorBehavior.State { get; }

        public void Enter()
        {
            Enter(State);
        }

        async Task<IActorBehavior> IActorBehavior.Execute()
        {
            var nextBehavior = await Execute();
            return nextBehavior;
        }

        public Task<ActorBehavior<T>> Execute()
        {
            return null;
        }
    }
}