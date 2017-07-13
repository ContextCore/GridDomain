using System;
using System.Collections.Generic;
using System.Linq;

namespace GridDomain.Node.Actors {
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
}