using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridDomain.Node.Actors
{

    public interface IActorState
    {
        string Name { get; }
    }

    public interface IActorBehavior
    {
        IActorState State { get; }
        void Enter();
        Task<IActorBehavior> Execute();
    }

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

    public class BehaviorBag
    {
        private readonly Queue<string> _behaviorHistory = new Queue<string>();
        private readonly Action<Action> _become;
        private readonly Action _unbecome;
        private readonly int _historyLimit;

        public string Current => _behaviorHistory.Any() ? _behaviorHistory.Last() : "Default behavior";
        public IReadOnlyCollection<string> History => _behaviorHistory;

        public BehaviorBag(Action<Action> become, Action unbecome, int historyLimit = 10)
        {
            _historyLimit = historyLimit;
            _unbecome = unbecome;
            _become = become;
        }

        public virtual void Become(Action act, string name)
        {
            AddToQueue(name);
            _become(act);
        }

        private void AddToQueue(string name)
        {
            if(_behaviorHistory.Count > _historyLimit)
                _behaviorHistory.Dequeue();
            _behaviorHistory.Enqueue(name);
        }

        public virtual string Unbecome()
        {
            _unbecome();
            var newBehavior = Current;
            AddToQueue(newBehavior);
            return newBehavior;
        }
    }

    public class BehaviorStack : BehaviorBag
    {
        private readonly Stack<string> _behaviorStack = new Stack<string>();
        public IReadOnlyCollection<string> Stack => _behaviorStack;

        public BehaviorStack(Action<Action> become, Action unbecome, int historyLimit = 10):base(become,unbecome,historyLimit)
        {
        }

        public override void Become(Action act, string name)
        {
            _behaviorStack.Push(name);
            base.Become(act,name);
        }


        public override string Unbecome()
        {
            _behaviorStack.Pop();
            return base.Unbecome();
        }
    }
}