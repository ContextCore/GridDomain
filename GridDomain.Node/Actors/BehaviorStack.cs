using System;
using System.Collections.Generic;

namespace GridDomain.Node.Actors
{
    public class BehaviorStack
    {
        private readonly Stack<string> _history = new Stack<string>();
        private readonly Action<Action> _become;
        private readonly Action _unbecome;

        public string Current => _history.Count > 0 ? _history.Peek() : "Default behavior";
        public IReadOnlyCollection<string> History => _history;

        public BehaviorStack(Action<Action> become, Action unbecome)
        {
            _unbecome = unbecome;
            _become = become;
        }

        public void Become(Action act, string name)
        {
            _history.Push(name);
            _become(act);
        }

        public string Unbecome()
        {
            _unbecome();
            return _history.Pop();
        }
    }
}