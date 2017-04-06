using System;
using System.Collections.Generic;
using System.Text;

namespace GridDomain.Node.Actors
{
    public class BehaviorStack
    {
        private readonly Stack<string> _behaviorStack = new Stack<string>();
        private readonly Queue<string> _behaviorHistory = new Queue<string>();
        private readonly Action<Action> _become;
        private readonly Action _unbecome;
        private readonly int _historyLimit;

        public string Current => _behaviorStack.Count > 0 ? _behaviorStack.Peek() : "Default behavior";
        public IReadOnlyCollection<string> History => _behaviorHistory;
        public IReadOnlyCollection<string> Stack => _behaviorStack;

        public BehaviorStack(Action<Action> become, Action unbecome, int historyLimit = 10)
        {
            _historyLimit = historyLimit;
            _unbecome = unbecome;
            _become = become;
        }

        public void Become(Action act, string name)
        {
            _behaviorStack.Push(name);
            AddToQueue(Current);
            _become(act);
        }

        private void AddToQueue(string name)
        {
            if (_behaviorHistory.Count > _historyLimit)
                _behaviorHistory.Dequeue();
            _behaviorHistory.Enqueue(name);
        }

        public string Unbecome()
        {
            _unbecome();
            _behaviorStack.Pop();
            var newBehavior = Current;
            AddToQueue(newBehavior);
            return newBehavior;
        }
    }
}